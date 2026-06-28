using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Profil;
using ReseauUniversitaire.Helpers;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProfilController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/profil/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMonProfil()
    {
        var userId = User.GetUserId();
        return await GetProfilParId(userId);
    }

    // GET /api/profil/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfilParId(int id)
    {
        var utilisateur = await _context.Utilisateurs
            .Include(u => u.Filiere)
            .Include(u => u.Publications)
            .Include(u => u.Abonnements)
            .Include(u => u.Abonnes)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (utilisateur == null)
            return NotFound();

        var dto = new ProfilDto
        {
            Id = utilisateur.Id,
            Nom = utilisateur.Nom,
            Prenom = utilisateur.Prenom,
            Email = utilisateur.Email,
            PhotoUrl = utilisateur.PhotoUrl,
            Bio = utilisateur.Bio,
            FiliereNom = utilisateur.Filiere?.Nom,
            DateInscription = utilisateur.DateInscription,
            NbPublications = utilisateur.Publications.Count,
            NbAbonnes = utilisateur.Abonnes.Count,
            NbAbonnements = utilisateur.Abonnements.Count
        };

        return Ok(dto);
    }

    // PUT /api/profil/me
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfil(UpdateProfilDto dto)
    {
        var userId = User.GetUserId();
        var utilisateur = await _context.Utilisateurs.FindAsync(userId);

        if (utilisateur == null)
            return NotFound();

        if (dto.Nom != null) utilisateur.Nom = dto.Nom;
        if (dto.Prenom != null) utilisateur.Prenom = dto.Prenom;
        if (dto.Bio != null) utilisateur.Bio = dto.Bio;
        if (dto.PhotoUrl != null) utilisateur.PhotoUrl = dto.PhotoUrl;
        if (dto.FiliereId != null) utilisateur.FiliereId = dto.FiliereId;

        await _context.SaveChangesAsync();

        return await GetMonProfil();
    }
}