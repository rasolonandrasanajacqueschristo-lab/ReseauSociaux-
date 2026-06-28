using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Admin;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/admin/stats
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = new AdminStatsDto
        {
            NbUtilisateurs = await _context.Utilisateurs.CountAsync(),
            NbPublications = await _context.Publications.CountAsync(),
            NbGroupes = await _context.Groupes.CountAsync(),
            NbRessources = await _context.Ressources.CountAsync(),
            NbSignalementsEnAttente = await _context.Signalements.CountAsync(s => !s.EstTraite)
        };

        return Ok(stats);
    }

    // GET /api/admin/utilisateurs
    [HttpGet("utilisateurs")]
    public async Task<IActionResult> GetUtilisateurs()
    {
        var utilisateurs = await _context.Utilisateurs
            .Select(u => new
            {
                u.Id,
                u.Nom,
                u.Prenom,
                u.Email,
                u.Role,
                u.EstActif,
                u.DateInscription
            })
            .ToListAsync();

        return Ok(utilisateurs);
    }

    // PUT /api/admin/utilisateurs/5/toggle-actif
    [HttpPut("utilisateurs/{id}/toggle-actif")]
    public async Task<IActionResult> ToggleActif(int id)
    {
        var utilisateur = await _context.Utilisateurs.FindAsync(id);
        if (utilisateur == null)
            return NotFound();

        utilisateur.EstActif = !utilisateur.EstActif;
        await _context.SaveChangesAsync();

        return Ok(new { estActif = utilisateur.EstActif });
    }

    // GET /api/admin/signalements
    [HttpGet("signalements")]
    public async Task<IActionResult> GetSignalements()
    {
        var signalements = await _context.Signalements
            .Include(s => s.Auteur)
            .Include(s => s.Publication)
            .Where(s => !s.EstTraite)
            .Select(s => new
            {
                s.Id,
                s.Raison,
                s.DateSignalement,
                AuteurNom = s.Auteur.Nom,
                PublicationContenu = s.Publication.Contenu,
                s.PublicationId
            })
            .ToListAsync();

        return Ok(signalements);
    }

    // PUT /api/admin/signalements/5/traiter
    [HttpPut("signalements/{id}/traiter")]
    public async Task<IActionResult> TraiterSignalement(int id, [FromQuery] bool supprimerPublication = false)
    {
        var signalement = await _context.Signalements
            .Include(s => s.Publication)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (signalement == null)
            return NotFound();

        signalement.EstTraite = true;

        if (supprimerPublication)
            _context.Publications.Remove(signalement.Publication);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Signalement traité." });
    }
}