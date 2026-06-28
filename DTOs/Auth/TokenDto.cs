namespace ReseauUniversitaire.DTOs.Auth;

public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public UtilisateurInfoDto Utilisateur { get; set; } = null!;
}

public class UtilisateurInfoDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}