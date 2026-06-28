namespace ReseauUniversitaire.DTOs.Commentaire;

public class CreateCommentaireDto
{
    public string Contenu { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}