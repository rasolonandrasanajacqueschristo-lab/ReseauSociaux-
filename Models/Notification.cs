namespace ReseauUniversitaire.Models;

public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool EstLue { get; set; } = false;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public string? LienUrl { get; set; }

    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; } = null!;
}