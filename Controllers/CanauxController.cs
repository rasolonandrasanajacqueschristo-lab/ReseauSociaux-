using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Canal;
using ReseauUniversitaire.Helpers;
using ReseauUniversitaire.Hubs;
using ReseauUniversitaire.Models;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/groupes/{groupeId}/canaux")]
[Authorize]
public class CanauxController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<GroupChatHub> _hubContext;

    public CanauxController(ApplicationDbContext context, IHubContext<GroupChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    private async Task<bool> EstMembre(int groupeId, int userId)
    {
        return await _context.MembresGroupe
            .AnyAsync(m => m.GroupeId == groupeId && m.UtilisateurId == userId && !m.DemandeEnAttente);
    }

    private async Task<bool> EstAdmin(int groupeId, int userId)
    {
        return await _context.MembresGroupe
            .AnyAsync(m => m.GroupeId == groupeId && m.UtilisateurId == userId && m.Role == "Admin");
    }

    // GET /api/groupes/5/canaux
    [HttpGet]
    public async Task<IActionResult> GetAll(int groupeId)
    {
        var userId = User.GetUserId();
        if (!await EstMembre(groupeId, userId))
            return Forbid();

        var canaux = await _context.Canaux
            .Where(c => c.GroupeId == groupeId)
            .Select(c => new CanalDto
            {
                Id = c.Id,
                Nom = c.Nom,
                Description = c.Description,
                EstAdminSeulement = c.EstAdminSeulement,
                GroupeId = c.GroupeId
            })
            .ToListAsync();

        return Ok(canaux);
    }

    // POST /api/groupes/5/canaux — créer un canal (admin seulement)
    [HttpPost]
    public async Task<IActionResult> Create(int groupeId, CreateCanalDto dto)
    {
        var userId = User.GetUserId();
        if (!await EstAdmin(groupeId, userId))
            return Forbid();

        var canal = new Canal
        {
            GroupeId = groupeId,
            Nom = dto.Nom,
            Description = dto.Description,
            EstAdminSeulement = dto.EstAdminSeulement
        };

        _context.Canaux.Add(canal);
        await _context.SaveChangesAsync();

        return Ok(canal);
    }

    // GET /api/groupes/5/canaux/3/messages
    [HttpGet("{canalId}/messages")]
    public async Task<IActionResult> GetMessages(int groupeId, int canalId)
    {
        var userId = User.GetUserId();
        if (!await EstMembre(groupeId, userId))
            return Forbid();

        var messages = await _context.MessagesCanaux
            .Where(m => m.CanalId == canalId)
            .Include(m => m.Expediteur)
            .OrderBy(m => m.DateEnvoi)
            .Select(m => new MessageCanalDto
            {
                Id = m.Id,
                Contenu = m.Contenu,
                FichierUrl = m.FichierUrl,
                DateEnvoi = m.DateEnvoi,
                ExpediteurId = m.ExpediteurId,
                ExpediteurNom = m.Expediteur.Nom,
                ExpediteurPrenom = m.Expediteur.Prenom,
                ExpediteurPhotoUrl = m.Expediteur.PhotoUrl
            })
            .ToListAsync();

        return Ok(messages);
    }

    // POST /api/groupes/5/canaux/3/messages — envoyer un message
    [HttpPost("{canalId}/messages")]
    public async Task<IActionResult> EnvoyerMessage(int groupeId, int canalId, EnvoyerMessageCanalDto dto)
    {
        var userId = User.GetUserId();
        if (!await EstMembre(groupeId, userId))
            return Forbid();

        var canal = await _context.Canaux.FindAsync(canalId);
        if (canal == null || canal.GroupeId != groupeId)
            return NotFound();

        if (canal.EstAdminSeulement && !await EstAdmin(groupeId, userId))
            return Forbid();

        var message = new MessageCanal
        {
            CanalId = canalId,
            ExpediteurId = userId,
            Contenu = dto.Contenu,
            FichierUrl = dto.FichierUrl
        };

        _context.MessagesCanaux.Add(message);
        await _context.SaveChangesAsync();

        var expediteur = await _context.Utilisateurs.FindAsync(userId);

        var messageDto = new MessageCanalDto
        {
            Id = message.Id,
            Contenu = message.Contenu,
            FichierUrl = message.FichierUrl,
            DateEnvoi = message.DateEnvoi,
            ExpediteurId = userId,
            ExpediteurNom = expediteur!.Nom,
            ExpediteurPrenom = expediteur.Prenom,
            ExpediteurPhotoUrl = expediteur.PhotoUrl
        };

        // Diffuse à tous les membres connectés à ce canal
        await _hubContext.Clients.Group($"canal-{canalId}")
            .SendAsync("NouveauMessageCanal", messageDto);

        return Ok(messageDto);
    }
}