namespace ReseauUniversitaire.DTOs.Commentaire;

public class CommentaireDto
{
    public int Id { get; set; }
    public string Contenu { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public int AuteurId { get; set; }
    public string AuteurNom { get; set; } = string.Empty;
    public string AuteurPrenom { get; set; } = string.Empty;
    public string? AuteurPhotoUrl { get; set; }
    public int? ParentId { get; set; }
    public List<CommentaireDto> Reponses { get; set; } = new();
}