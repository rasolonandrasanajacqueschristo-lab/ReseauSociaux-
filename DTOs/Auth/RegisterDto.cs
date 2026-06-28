namespace ReseauUniversitaire.DTOs.Auth;

public class RegisterDto
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MotDePasse { get; set; } = string.Empty;
    public int? FiliereId { get; set; }
}