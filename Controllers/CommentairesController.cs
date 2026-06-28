using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Commentaire;
using ReseauUniversitaire.Helpers;
using ReseauUniversitaire.Hubs;
using Microsoft.AspNetCore.SignalR;
using ReseauUniversitaire.Models;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/publications/{publicationId}/commentaires")]
[Authorize]
public class CommentairesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public CommentairesController(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // GET /api/publications/5/commentaires
    [HttpGet]
    public async Task<IActionResult> GetAll(int publicationId)
    {
        var commentaires = await _context.Commentaires
            .Where(c => c.PublicationId == publicationId && c.ParentId == null)
            .Include(c => c.Auteur)
            .Include(c => c.Reponses).ThenInclude(r => r.Auteur)
            .OrderBy(c => c.DateCreation)
            .ToListAsync();

        var result = commentaires.Select(MapToDto).ToList();
        return Ok(result);
    }

    // POST /api/publications/5/commentaires
    [HttpPost]
    public async Task<IActionResult> Create(int publicationId, CreateCommentaireDto dto)
    {
        var userId = User.GetUserId();

        var publication = await _context.Publications
            .Include(p => p.Auteur)
            .FirstOrDefaultAsync(p => p.Id == publicationId);

        if (publication == null)
            return NotFound();

        var commentaire = new ReseauUniversitaire.Models.Commentaire
        {
            Contenu = dto.Contenu,
            PublicationId = publicationId,
            AuteurId = userId,
            ParentId = dto.ParentId
        };

        _context.Commentaires.Add(commentaire);
        await _context.SaveChangesAsync();

        // Notifier l'auteur de la publication (sauf s'il commente lui-même)
        if (publication.AuteurId != userId)
        {
            var auteurCommentaire = await _context.Utilisateurs.FindAsync(userId);

            var notification = new Notification
            {
                UtilisateurId = publication.AuteurId,
                Message = $"{auteurCommentaire!.Prenom} a commenté votre publication",
                Type = "Commentaire",
                LienUrl = $"/publications/{publicationId}"
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(publication.AuteurId.ToString())
                .SendAsync("NouvelleNotification", new
                {
                    notification.Id,
                    notification.Message,
                    notification.Type,
                    notification.LienUrl,
                    notification.DateCreation
                });
        }

        var commentaireComplet = await _context.Commentaires
            .Include(c => c.Auteur)
            .FirstAsync(c => c.Id == commentaire.Id);

        return Ok(MapToDto(commentaireComplet));
    }

    // DELETE /api/publications/5/commentaires/3
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int publicationId, int id)
    {
        var userId = User.GetUserId();
        var commentaire = await _context.Commentaires.FindAsync(id);

        if (commentaire == null)
            return NotFound();

        if (commentaire.AuteurId != userId)
            return Forbid();

        _context.Commentaires.Remove(commentaire);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static CommentaireDto MapToDto(ReseauUniversitaire.Models.Commentaire c)
    {
        return new CommentaireDto
        {
            Id = c.Id,
            Contenu = c.Contenu,
            DateCreation = c.DateCreation,
            AuteurId = c.AuteurId,
            AuteurNom = c.Auteur.Nom,
            AuteurPrenom = c.Auteur.Prenom,
            AuteurPhotoUrl = c.Auteur.PhotoUrl,
            ParentId = c.ParentId,
            Reponses = c.Reponses?.Select(MapToDto).ToList() ?? new()
        };
    }
}