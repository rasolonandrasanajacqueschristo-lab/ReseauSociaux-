using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Auth;
using ReseauUniversitaire.Models;

namespace ReseauUniversitaire.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;

    public AuthService(ApplicationDbContext context, ITokenService tokenService, IConfiguration config)
    {
        _context = context;
        _tokenService = tokenService;
        _config = config;
    }

    public async Task<TokenDto?> RegisterAsync(RegisterDto dto)
    {
        var emailExiste = await _context.Utilisateurs
            .AnyAsync(u => u.Email == dto.Email);

        if (emailExiste)
            return null; // email déjà utilisé

        var utilisateur = new Utilisateur
        {
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Email = dto.Email,
            MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(dto.MotDePasse),
            FiliereId = dto.FiliereId,
            Role = "Etudiant"
        };

        _context.Utilisateurs.Add(utilisateur);
        await _context.SaveChangesAsync();

        return GenererReponseToken(utilisateur);
    }

    public async Task<TokenDto?> LoginAsync(LoginDto dto)
    {
        var utilisateur = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (utilisateur == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.MotDePasse, utilisateur.MotDePasseHash))
            return null;

        if (!utilisateur.EstActif)
            return null;

        return GenererReponseToken(utilisateur);
    }

    private TokenDto GenererReponseToken(Utilisateur utilisateur)
    {
        var token = _tokenService.GenererToken(utilisateur);
        var expireMinutes = int.Parse(_config["Jwt:ExpireMinutes"]!);

        return new TokenDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(expireMinutes),
            Utilisateur = new UtilisateurInfoDto
            {
                Id = utilisateur.Id,
                Nom = utilisateur.Nom,
                Prenom = utilisateur.Prenom,
                Email = utilisateur.Email,
                Role = utilisateur.Role,
                PhotoUrl = utilisateur.PhotoUrl
            }
        };
    }
}