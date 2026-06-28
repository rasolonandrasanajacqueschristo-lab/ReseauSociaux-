using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Message;
using ReseauUniversitaire.Helpers;
using ReseauUniversitaire.Hubs;
using ReseauUniversitaire.Models;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessagesController(ApplicationDbContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // GET /api/messages/conversations — liste de toutes mes conversations
    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var userId = User.GetUserId();

        var conversations = await _context.Conversations
            .Where(c => c.Participant1Id == userId || c.Participant2Id == userId)
            .Include(c => c.Participant1)
            .Include(c => c.Participant2)
            .Include(c => c.Messages)
            .ToListAsync();

        var result = conversations.Select(c =>
        {
            var autre = c.Participant1Id == userId ? c.Participant2 : c.Participant1;
            var dernierMessage = c.Messages.OrderByDescending(m => m.DateEnvoi).FirstOrDefault();

            return new ConversationDto
            {
                Id = c.Id,
                AutreUtilisateurId = autre.Id,
                AutreUtilisateurNom = autre.Nom,
                AutreUtilisateurPrenom = autre.Prenom,
                AutreUtilisateurPhotoUrl = autre.PhotoUrl,
                DernierMessage = dernierMessage?.Contenu,
                DateDernierMessage = dernierMessage?.DateEnvoi,
                NbNonLus = c.Messages.Count(m => !m.EstLu && m.ExpediteurId != userId)
            };
        })
        .OrderByDescending(c => c.DateDernierMessage)
        .ToList();

        return Ok(result);
    }

    // GET /api/messages/conversation/5 — messages avec un utilisateur précis
    [HttpGet("conversation/{autreUserId}")]
    public async Task<IActionResult> GetMessagesAvecUtilisateur(int autreUserId)
    {
        var userId = User.GetUserId();

        if (autreUserId == userId)
            return BadRequest(new { message = "Vous ne pouvez pas avoir une conversation avec vous-même." });

        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c =>
                (c.Participant1Id == userId && c.Participant2Id == autreUserId) ||
                (c.Participant1Id == autreUserId && c.Participant2Id == userId));

        if (conversation == null)
            return Ok(new List<MessageDto>()); // pas encore de conversation

        var messages = await _context.Messages
            .Where(m => m.ConversationId == conversation.Id)
            .Include(m => m.Expediteur)
            .OrderBy(m => m.DateEnvoi)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                Contenu = m.Contenu,
                FichierUrl = m.FichierUrl,
                EstLu = m.EstLu,
                DateEnvoi = m.DateEnvoi,
                ExpediteurId = m.ExpediteurId,
                ExpediteurNom = m.Expediteur.Nom,
                ExpediteurPrenom = m.Expediteur.Prenom
            })
            .ToListAsync();

        // Marquer comme lus les messages reçus
        var messagesNonLus = await _context.Messages
            .Where(m => m.ConversationId == conversation.Id
                && m.ExpediteurId != userId
                && !m.EstLu)
            .ToListAsync();

        foreach (var msg in messagesNonLus)
            msg.EstLu = true;

        if (messagesNonLus.Any())
            await _context.SaveChangesAsync();

        return Ok(messages);
    }

    // POST /api/messages — envoyer un message
    [HttpPost]
    public async Task<IActionResult> Envoyer(SendMessageDto dto)
    {
        var userId = User.GetUserId();

        if (dto.DestinataireId == userId)
            return BadRequest(new { message = "Vous ne pouvez pas vous envoyer un message à vous-même." });

        var destinataireExiste = await _context.Utilisateurs.AnyAsync(u => u.Id == dto.DestinataireId);
        if (!destinataireExiste)
            return NotFound(new { message = "Destinataire introuvable." });

        // Cherche une conversation existante, ou la crée
        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c =>
                (c.Participant1Id == userId && c.Participant2Id == dto.DestinataireId) ||
                (c.Participant1Id == dto.DestinataireId && c.Participant2Id == userId));

        if (conversation == null)
        {
            conversation = new Conversation
            {
                Participant1Id = userId,
                Participant2Id = dto.DestinataireId
            };
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
        }

        var message = new Message
        {
            ConversationId = conversation.Id,
            ExpediteurId = userId,
            Contenu = dto.Contenu,
            FichierUrl = dto.FichierUrl
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var expediteur = await _context.Utilisateurs.FindAsync(userId);

        var messageDto = new MessageDto
        {
            Id = message.Id,
            Contenu = message.Contenu,
            FichierUrl = message.FichierUrl,
            EstLu = false,
            DateEnvoi = message.DateEnvoi,
            ExpediteurId = userId,
            ExpediteurNom = expediteur!.Nom,
            ExpediteurPrenom = expediteur.Prenom
        };

        // 🔥 Envoi temps réel via SignalR au destinataire
        await _hubContext.Clients.User(dto.DestinataireId.ToString())
            .SendAsync("ReceiveMessage", messageDto);

        return Ok(messageDto);
    }
}