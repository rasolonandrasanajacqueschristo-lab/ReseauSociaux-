namespace ReseauUniversitaire.Models;

public class Evenement
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Lieu { get; set; } = string.Empty;
    public DateTime DateEvenement { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public int OrganisateurId { get; set; }
    public Utilisateur Organisateur { get; set; } = null!;
    public int? GroupeId { get; set; }
    public Groupe? Groupe { get; set; }
}