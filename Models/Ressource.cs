namespace ReseauUniversitaire.Models;

public class Ressource
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FichierUrl { get; set; } = string.Empty;
    public string TypeFichier { get; set; } = string.Empty;
    public string Matiere { get; set; } = string.Empty;
    public int NbTelechargements { get; set; } = 0;
    public DateTime DateUpload { get; set; } = DateTime.UtcNow;

    public int AuteurId { get; set; }
    public Utilisateur Auteur { get; set; } = null!;
    public int? FiliereId { get; set; }
    public Filiere? Filiere { get; set; }
    public ICollection<EvaluationRessource> Evaluations { get; set; } = new List<EvaluationRessource>();
}