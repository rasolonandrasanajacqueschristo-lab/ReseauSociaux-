using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Evenement;
using ReseauUniversitaire.Helpers;
using ReseauUniversitaire.Models;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EvenementsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EvenementsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/evenements
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var evenements = await _context.Evenements
            .Include(e => e.Organisateur)
            .Include(e => e.Groupe)
            .Where(e => e.DateEvenement >= DateTime.UtcNow)
            .OrderBy(e => e.DateEvenement)
            .Select(e => new EvenementDto
            {
                Id = e.Id,
                Titre = e.Titre,
                Description = e.Description,
                Lieu = e.Lieu,
                DateEvenement = e.DateEvenement,
                OrganisateurId = e.OrganisateurId,
                OrganisateurNom = e.Organisateur.Nom,
                OrganisateurPrenom = e.Organisateur.Prenom,
                GroupeNom = e.Groupe == null ? null : e.Groupe.Nom
            })
            .ToListAsync();

        return Ok(evenements);
    }

    // GET /api/evenements/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var evenement = await _context.Evenements
            .Include(e => e.Organisateur)
            .Include(e => e.Groupe)
            .Where(e => e.Id == id)
            .Select(e => new EvenementDto
            {
                Id = e.Id,
                Titre = e.Titre,
                Description = e.Description,
                Lieu = e.Lieu,
                DateEvenement = e.DateEvenement,
                OrganisateurId = e.OrganisateurId,
                OrganisateurNom = e.Organisateur.Nom,
                OrganisateurPrenom = e.Organisateur.Prenom,
                GroupeNom = e.Groupe == null ? null : e.Groupe.Nom
            })
            .FirstOrDefaultAsync();

        if (evenement == null)
            return NotFound();

        return Ok(evenement);
    }

    // POST /api/evenements
    [HttpPost]
    public async Task<IActionResult> Create(CreateEvenementDto dto)
    {
        var userId = User.GetUserId();

        var evenement = new Evenement
        {
            Titre = dto.Titre,
            Description = dto.Description,
            Lieu = dto.Lieu,
            DateEvenement = dto.DateEvenement,
            GroupeId = dto.GroupeId,
            OrganisateurId = userId
        };

        _context.Evenements.Add(evenement);
        await _context.SaveChangesAsync();

        return Ok(evenement);
    }

    // DELETE /api/evenements/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var evenement = await _context.Evenements.FindAsync(id);

        if (evenement == null)
            return NotFound();

        if (evenement.OrganisateurId != userId)
            return Forbid();

        _context.Evenements.Remove(evenement);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}