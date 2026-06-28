namespace ReseauUniversitaire.Models;

public class Message
{
    public int Id { get; set; }
    public string Contenu { get; set; } = string.Empty;
    public string? FichierUrl { get; set; }
    public bool EstLu { get; set; } = false;
    public DateTime DateEnvoi { get; set; } = DateTime.UtcNow;

    public int ExpediteurId { get; set; }
    public Utilisateur Expediteur { get; set; } = null!;
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;
}