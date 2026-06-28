using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Notification;
using ReseauUniversitaire.Helpers;
using ReseauUniversitaire.Models;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NotificationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/notifications
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.GetUserId();

        var notifications = await _context.Notifications
            .Where(n => n.UtilisateurId == userId)
            .OrderByDescending(n => n.DateCreation)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                Type = n.Type,
                EstLue = n.EstLue,
                DateCreation = n.DateCreation,
                LienUrl = n.LienUrl
            })
            .ToListAsync();

        return Ok(notifications);
    }

    // GET /api/notifications/non-lues-count
    [HttpGet("non-lues-count")]
    public async Task<IActionResult> GetNonLuesCount()
    {
        var userId = User.GetUserId();

        var count = await _context.Notifications
            .CountAsync(n => n.UtilisateurId == userId && !n.EstLue);

        return Ok(new { count });
    }

    // PUT /api/notifications/5/marquer-lue
    [HttpPut("{id}/marquer-lue")]
    public async Task<IActionResult> MarquerLue(int id)
    {
        var userId = User.GetUserId();
        var notification = await _context.Notifications.FindAsync(id);

        if (notification == null || notification.UtilisateurId != userId)
            return NotFound();

        notification.EstLue = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT /api/notifications/marquer-toutes-lues
    [HttpPut("marquer-toutes-lues")]
    public async Task<IActionResult> MarquerToutesLues()
    {
        var userId = User.GetUserId();

        var notifications = await _context.Notifications
            .Where(n => n.UtilisateurId == userId && !n.EstLue)
            .ToListAsync();

        foreach (var n in notifications)
            n.EstLue = true;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}