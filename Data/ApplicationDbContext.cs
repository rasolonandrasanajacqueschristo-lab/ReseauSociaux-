using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Models;

namespace ReseauUniversitaire.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<Filiere> Filieres { get; set; }
    public DbSet<Publication> Publications { get; set; }
    public DbSet<Commentaire> Commentaires { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Groupe> Groupes { get; set; }
    public DbSet<MembreGroupe> MembresGroupe { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Ressource> Ressources { get; set; }
    public DbSet<EvaluationRessource> EvaluationsRessource { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Signalement> Signalements { get; set; }
    public DbSet<Evenement> Evenements { get; set; }
    public DbSet<Abonnement> Abonnements { get; set; }
    public DbSet<Canal> Canaux { get; set; }
    public DbSet<MessageCanal> MessagesCanaux { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Participant1)
            .WithMany()
            .HasForeignKey(c => c.Participant1Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Participant2)
            .WithMany()
            .HasForeignKey(c => c.Participant2Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Abonnement>()
            .HasOne(a => a.Abonne)
            .WithMany(u => u.Abonnements)
            .HasForeignKey(a => a.AbonneId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Abonnement>()
            .HasOne(a => a.AbonnementUtilisateur)
            .WithMany(u => u.Abonnes)
            .HasForeignKey(a => a.AbonnementId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Utilisateur>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Commentaire>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Reponses)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}