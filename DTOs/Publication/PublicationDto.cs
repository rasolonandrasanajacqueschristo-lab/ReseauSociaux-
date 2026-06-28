namespace ReseauUniversitaire.DTOs.Publication;

public class PublicationDto
{
    public int Id { get; set; }
    public string Contenu { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? FichierUrl { get; set; }
    public string? FichierNom { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
    public DateTime DateCreation { get; set; }
    public int AuteurId { get; set; }
    public string AuteurNom { get; set; } = string.Empty;
    public string AuteurPrenom { get; set; } = string.Empty;
    public string? AuteurPhotoUrl { get; set; }
    public int NbLikes { get; set; }
    public int NbCommentaires { get; set; }
    public bool EstLikeParMoi { get; set; }
}