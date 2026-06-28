namespace ReseauUniversitaire.DTOs.Ressource;

public class CreateRessourceDto
{
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FichierUrl { get; set; } = string.Empty;
    public string TypeFichier { get; set; } = string.Empty;
    public string Matiere { get; set; } = string.Empty;
    public int? FiliereId { get; set; }
}