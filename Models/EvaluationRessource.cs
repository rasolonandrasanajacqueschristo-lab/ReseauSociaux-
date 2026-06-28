namespace ReseauUniversitaire.Models;

public class EvaluationRessource
{
    public int Id { get; set; }
    public int Note { get; set; }
    public string? Commentaire { get; set; }
    public DateTime DateEvaluation { get; set; } = DateTime.UtcNow;

    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; } = null!;
    public int RessourceId { get; set; }
    public Ressource Ressource { get; set; } = null!;
}