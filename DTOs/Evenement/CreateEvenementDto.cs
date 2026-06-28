namespace ReseauUniversitaire.DTOs.Evenement;

public class CreateEvenementDto
{
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Lieu { get; set; } = string.Empty;
    public DateTime DateEvenement { get; set; }
    public int? GroupeId { get; set; }
}