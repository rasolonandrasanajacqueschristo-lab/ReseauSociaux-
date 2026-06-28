namespace ReseauUniversitaire.Models;

public class Utilisateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MotDePasseHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Etudiant";
    public string? PhotoUrl { get; set; }
    public string? Bio { get; set; }
    public bool EstActif { get; set; } = true;
    public DateTime DateInscription { get; set; } = DateTime.UtcNow;

    public int? FiliereId { get; set; }
    public Filiere? Filiere { get; set; }
    public ICollection<Publication> Publications { get; set; } = new List<Publication>();
    public ICollection<Message> MessagesEnvoyes { get; set; } = new List<Message>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<MembreGroupe> Groupes { get; set; } = new List<MembreGroupe>();
    public ICollection<Abonnement> Abonnements { get; set; } = new List<Abonnement>();
    public ICollection<Abonnement> Abonnes { get; set; } = new List<Abonnement>();
}