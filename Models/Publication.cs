namespace ReseauUniversitaire.Models;

public class Publication
{
    public int Id { get; set; }
    public string Contenu { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? FichierUrl { get; set; }
    public string? FichierNom { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public bool EstSignale { get; set; } = false;

    public int AuteurId { get; set; }
    public Utilisateur Auteur { get; set; } = null!;
    public int? GroupeId { get; set; }
    public Groupe? Groupe { get; set; }
    public ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Signalement> Signalements { get; set; } = new List<Signalement>();
}