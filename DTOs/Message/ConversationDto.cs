namespace ReseauUniversitaire.DTOs.Message;

public class ConversationDto
{
    public int Id { get; set; }
    public int AutreUtilisateurId { get; set; }
    public string AutreUtilisateurNom { get; set; } = string.Empty;
    public string AutreUtilisateurPrenom { get; set; } = string.Empty;
    public string? AutreUtilisateurPhotoUrl { get; set; }
    public string? DernierMessage { get; set; }
    public DateTime? DateDernierMessage { get; set; }
    public int NbNonLus { get; set; }
}