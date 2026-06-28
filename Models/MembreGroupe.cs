namespace ReseauUniversitaire.Models;

public class MembreGroupe
{
    public int Id { get; set; }
    public string Role { get; set; } = "Membre";
    public DateTime DateAdhesion { get; set; } = DateTime.UtcNow;
    public bool DemandeEnAttente { get; set; } = false;

    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; } = null!;
    public int GroupeId { get; set; }
    public Groupe Groupe { get; set; } = null!;
}