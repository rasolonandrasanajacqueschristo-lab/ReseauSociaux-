namespace ReseauUniversitaire.Models;

public class Conversation
{
    public int Id { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public int Participant1Id { get; set; }
    public Utilisateur Participant1 { get; set; } = null!;
    public int Participant2Id { get; set; }
    public Utilisateur Participant2 { get; set; } = null!;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}