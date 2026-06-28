namespace ReseauUniversitaire.DTOs.Publication;

public class CreatePublicationDto
{
    public string Contenu { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? FichierUrl { get; set; }
    public string? FichierNom { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
    public int? GroupeId { get; set; }
}