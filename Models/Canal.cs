namespace ReseauUniversitaire.Models;

public class Canal
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool EstAdminSeulement { get; set; } = false;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public int GroupeId { get; set; }
    public Groupe Groupe { get; set; } = null!;
    public ICollection<MessageCanal> Messages { get; set; } = new List<MessageCanal>();
}