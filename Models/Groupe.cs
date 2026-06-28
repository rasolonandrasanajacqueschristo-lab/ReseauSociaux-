namespace ReseauUniversitaire.Models;

public class Groupe
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool EstPrive { get; set; } = false;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public int? FiliereId { get; set; }
    public Filiere? Filiere { get; set; }
    public ICollection<MembreGroupe> Membres { get; set; } = new List<MembreGroupe>();
    public ICollection<Publication> Publications { get; set; } = new List<Publication>();
    public ICollection<Canal> Canaux { get; set; } = new List<Canal>();
}