namespace ReseauUniversitaire.DTOs.Profil;

public class UpdateProfilDto
{
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public string? Bio { get; set; }
    public string? PhotoUrl { get; set; }
    public int? FiliereId { get; set; }
}