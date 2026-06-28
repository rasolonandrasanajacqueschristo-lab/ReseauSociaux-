namespace ReseauUniversitaire.Models;

public class Filiere
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Utilisateur> Etudiants { get; set; } = new List<Utilisateur>();
    public ICollection<Groupe> Groupes { get; set; } = new List<Groupe>();
}