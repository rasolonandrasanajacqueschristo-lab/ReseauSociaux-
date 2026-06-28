namespace ReseauUniversitaire.Models;

public class Commentaire
{
    public int Id { get; set; }
    public string Contenu { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public int AuteurId { get; set; }
    public Utilisateur Auteur { get; set; } = null!;
    public int PublicationId { get; set; }
    public Publication Publication { get; set; } = null!;

    public int? ParentId { get; set; }
    public Commentaire? Parent { get; set; }
    public ICollection<Commentaire> Reponses { get; set; } = new List<Commentaire>();
}