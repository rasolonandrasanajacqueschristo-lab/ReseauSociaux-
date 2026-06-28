namespace ReseauUniversitaire.Models;

public class Signalement
{
    public int Id { get; set; }
    public string Raison { get; set; } = string.Empty;
    public bool EstTraite { get; set; } = false;
    public DateTime DateSignalement { get; set; } = DateTime.UtcNow;

    public int AuteurId { get; set; }
    public Utilisateur Auteur { get; set; } = null!;
    public int PublicationId { get; set; }
    public Publication Publication { get; set; } = null!;
}