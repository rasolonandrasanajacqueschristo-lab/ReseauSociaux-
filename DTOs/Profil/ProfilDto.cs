namespace ReseauUniversitaire.DTOs.Profil;

public class ProfilDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? Bio { get; set; }
    public string? FiliereNom { get; set; }
    public DateTime DateInscription { get; set; }
    public int NbPublications { get; set; }
    public int NbAbonnes { get; set; }
    public int NbAbonnements { get; set; }
}