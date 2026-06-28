namespace ReseauUniversitaire.DTOs.Ressource;

public class RessourceDto
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FichierUrl { get; set; } = string.Empty;
    public string TypeFichier { get; set; } = string.Empty;
    public string Matiere { get; set; } = string.Empty;
    public int NbTelechargements { get; set; }
    public DateTime DateUpload { get; set; }
    public int AuteurId { get; set; }
    public string AuteurNom { get; set; } = string.Empty;
    public string AuteurPrenom { get; set; } = string.Empty;
    public string? FiliereNom { get; set; }
    public double NoteMoyenne { get; set; }
    public int NbEvaluations { get; set; }
}