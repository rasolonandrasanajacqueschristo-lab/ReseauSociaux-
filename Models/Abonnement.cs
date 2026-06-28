namespace ReseauUniversitaire.Models;

public class Abonnement
{
    public int Id { get; set; }
    public DateTime DateAbonnement { get; set; } = DateTime.UtcNow;

    public int AbonneId { get; set; }
    public Utilisateur Abonne { get; set; } = null!;
    public int AbonnementId { get; set; }
    public Utilisateur AbonnementUtilisateur { get; set; } = null!;
}