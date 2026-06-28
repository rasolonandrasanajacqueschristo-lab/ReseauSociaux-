namespace ReseauUniversitaire.Models;

public class MessageCanal
{
    public int Id { get; set; }
    public string Contenu { get; set; } = string.Empty;
    public string? FichierUrl { get; set; }
    public DateTime DateEnvoi { get; set; } = DateTime.UtcNow;

    public int CanalId { get; set; }
    public Canal Canal { get; set; } = null!;
    public int ExpediteurId { get; set; }
    public Utilisateur Expediteur { get; set; } = null!;
}