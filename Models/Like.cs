namespace ReseauUniversitaire.Models;

public class Like
{
    public int Id { get; set; }
    public DateTime DateLike { get; set; } = DateTime.UtcNow;

    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; } = null!;
    public int PublicationId { get; set; }
    public Publication Publication { get; set; } = null!;
}