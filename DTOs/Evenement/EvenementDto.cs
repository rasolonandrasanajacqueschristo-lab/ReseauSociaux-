namespace ReseauUniversitaire.DTOs.Evenement;

public class EvenementDto
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Lieu { get; set; } = string.Empty;
    public DateTime DateEvenement { get; set; }
    public int OrganisateurId { get; set; }
    public string OrganisateurNom { get; set; } = string.Empty;
    public string OrganisateurPrenom { get; set; } = string.Empty;
    public string? GroupeNom { get; set; }
}