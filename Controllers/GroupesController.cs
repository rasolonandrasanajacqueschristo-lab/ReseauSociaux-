using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Groupe;
using ReseauUniversitaire.Helpers;
using ReseauUniversitaire.Models;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GroupesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/groupes
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.GetUserId();

        var groupes = await _context.Groupes
            .Include(g => g.Filiere)
            .Include(g => g.Membres)
            .Select(g => new GroupeDto
            {
                Id = g.Id,
                Nom = g.Nom,
                Description = g.Description,
                EstPrive = g.EstPrive,
                FiliereNom = g.Filiere == null ? null : g.Filiere.Nom,
                NbMembres = g.Membres.Count,
                JeSuisMembre = g.Membres.Any(m => m.UtilisateurId == userId)
            })
            .ToListAsync();

        return Ok(groupes);
    }

    // GET /api/groupes/mes-groupes
    [HttpGet("mes-groupes")]
    public async Task<IActionResult> GetMesGroupes()
    {
        var userId = User.GetUserId();

        var groupes = await _context.MembresGroupe
            .Where(m => m.UtilisateurId == userId && !m.DemandeEnAttente)
            .Include(m => m.Groupe).ThenInclude(g => g.Filiere)
            .Include(m => m.Groupe).ThenInclude(g => g.Membres)
            .Select(m => new GroupeDto
            {
                Id = m.Groupe.Id,
                Nom = m.Groupe.Nom,
                Description = m.Groupe.Description,
                EstPrive = m.Groupe.EstPrive,
                FiliereNom = m.Groupe.Filiere == null ? null : m.Groupe.Filiere.Nom,
                NbMembres = m.Groupe.Membres.Count,
                JeSuisMembre = true
            })
            .ToListAsync();

        return Ok(groupes);
    }

    // POST /api/groupes
    [HttpPost]
    public async Task<IActionResult> Create(CreateGroupeDto dto)
    {
        var userId = User.GetUserId();

        var groupe = new Groupe
        {
            Nom = dto.Nom,
            Description = dto.Description,
            EstPrive = dto.EstPrive,
            FiliereId = dto.FiliereId
        };

        _context.Groupes.Add(groupe);
        await _context.SaveChangesAsync();

        // Le créateur devient automatiquement Admin du groupe
        _context.MembresGroupe.Add(new MembreGroupe
        {
            GroupeId = groupe.Id,
            UtilisateurId = userId,
            Role = "Admin"
        });

        // Créer les canaux par défaut
        _context.Canaux.AddRange(
            new Canal { GroupeId = groupe.Id, Nom = "général", Description = "Discussion libre" },
            new Canal { GroupeId = groupe.Id, Nom = "annonces", Description = "Annonces importantes", EstAdminSeulement = true },
            new Canal { GroupeId = groupe.Id, Nom = "devoirs", Description = "Partage de ressources et devoirs" }
        );

        await _context.SaveChangesAsync();

        return Ok(groupe);
    }

    // POST /api/groupes/5/rejoindre
    [HttpPost("{id}/rejoindre")]
    public async Task<IActionResult> Rejoindre(int id)
    {
        var userId = User.GetUserId();

        var groupe = await _context.Groupes.FindAsync(id);
        if (groupe == null)
            return NotFound();

        var dejaMembersexist = await _context.MembresGroupe
            .AnyAsync(m => m.GroupeId == id && m.UtilisateurId == userId);

        if (dejaMembersexist)
            return BadRequest(new { message = "Vous êtes déjà membre de ce groupe." });

        _context.MembresGroupe.Add(new MembreGroupe
        {
            GroupeId = id,
            UtilisateurId = userId,
            Role = "Membre",
            DemandeEnAttente = groupe.EstPrive
        });
        await _context.SaveChangesAsync();

        return Ok(new { message = groupe.EstPrive ? "Demande envoyée." : "Vous avez rejoint le groupe." });
    }

    // POST /api/groupes/5/quitter
    [HttpPost("{id}/quitter")]
    public async Task<IActionResult> Quitter(int id)
    {
        var userId = User.GetUserId();

        var membre = await _context.MembresGroupe
            .FirstOrDefaultAsync(m => m.GroupeId == id && m.UtilisateurId == userId);

        if (membre == null)
            return NotFound();

        _context.MembresGroupe.Remove(membre);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET /api/groupes/5/membres
    [HttpGet("{id}/membres")]
    public async Task<IActionResult> GetMembres(int id)
    {
        var membres = await _context.MembresGroupe
            .Where(m => m.GroupeId == id && !m.DemandeEnAttente)
            .Include(m => m.Utilisateur)
            .Select(m => new
            {
                m.Utilisateur.Id,
                m.Utilisateur.Nom,
                m.Utilisateur.Prenom,
                m.Utilisateur.PhotoUrl,
                m.Role
            })
            .ToListAsync();

        return Ok(membres);
    }

   // GET /api/groupes/5/demandes — demandes en attente (admin seulement)
[HttpGet("{id}/demandes")]
public async Task<IActionResult> GetDemandesEnAttente(int id)
{
    var userId = User.GetUserId();

    var estAdmin = await _context.MembresGroupe
        .AnyAsync(m => m.GroupeId == id && m.UtilisateurId == userId && m.Role == "Admin");

    if (!estAdmin)
        return Forbid();

    var demandes = await _context.MembresGroupe
        .Where(m => m.GroupeId == id && m.DemandeEnAttente)
        .Include(m => m.Utilisateur)
        .Select(m => new
        {
            MembreId = m.Id,
            UtilisateurId = m.Utilisateur.Id,
            Nom = m.Utilisateur.Nom,
            Prenom = m.Utilisateur.Prenom,
            PhotoUrl = m.Utilisateur.PhotoUrl
        })
        .ToListAsync();

    return Ok(demandes);
}
    // PUT /api/groupes/5/demandes/3/accepter
    [HttpPut("{id}/demandes/{membreId}/accepter")]
    public async Task<IActionResult> AccepterDemande(int id, int membreId)
    {
        var userId = User.GetUserId();

        var estAdmin = await _context.MembresGroupe
            .AnyAsync(m => m.GroupeId == id && m.UtilisateurId == userId && m.Role == "Admin");

        if (!estAdmin)
            return Forbid();

        var membre = await _context.MembresGroupe.FindAsync(membreId);
        if (membre == null || membre.GroupeId != id)
            return NotFound();

        membre.DemandeEnAttente = false;
        await _context.SaveChangesAsync();

        return Ok();
    }

    // DELETE /api/groupes/5/demandes/3/refuser
    [HttpDelete("{id}/demandes/{membreId}/refuser")]
    public async Task<IActionResult> RefuserDemande(int id, int membreId)
    {
        var userId = User.GetUserId();

        var estAdmin = await _context.MembresGroupe
            .AnyAsync(m => m.GroupeId == id && m.UtilisateurId == userId && m.Role == "Admin");

        if (!estAdmin)
            return Forbid();

        var membre = await _context.MembresGroupe.FindAsync(membreId);
        if (membre == null || membre.GroupeId != id)
            return NotFound();

        _context.MembresGroupe.Remove(membre);
        await _context.SaveChangesAsync();

        return Ok();
    }

  // POST /api/groupes/5/inviter/3
[HttpPost("{id}/inviter/{utilisateurId}")]
public async Task<IActionResult> Inviter(int id, int utilisateurId)
{
    var userId = User.GetUserId();

    var groupe = await _context.Groupes.FindAsync(id);
    if (groupe == null)
        return NotFound();

    var monMembre = await _context.MembresGroupe
        .FirstOrDefaultAsync(m => m.GroupeId == id && m.UtilisateurId == userId);

    if (monMembre == null)
        return Forbid(); // je ne suis même pas membre

    // Groupe privé → seul Admin peut inviter. Groupe public → tout membre peut inviter.
    if (groupe.EstPrive && monMembre.Role != "Admin")
        return Forbid();

    var dejaMembersexist = await _context.MembresGroupe
        .AnyAsync(m => m.GroupeId == id && m.UtilisateurId == utilisateurId);

    if (dejaMembersexist)
        return BadRequest(new { message = "Cette personne est déjà membre ou a déjà une demande en cours." });

    _context.MembresGroupe.Add(new MembreGroupe
    {
        GroupeId = id,
        UtilisateurId = utilisateurId,
        Role = "Membre",
        DemandeEnAttente = false
    });
    await _context.SaveChangesAsync();

    return Ok(new { message = "Membre ajouté avec succès." });
}

// DELETE /api/groupes/5/membres/3 — retirer un membre (admin seulement)
[HttpDelete("{id}/membres/{utilisateurId}")]
public async Task<IActionResult> RetirerMembre(int id, int utilisateurId)
{
    var userId = User.GetUserId();

    var estAdmin = await _context.MembresGroupe
        .AnyAsync(m => m.GroupeId == id && m.UtilisateurId == userId && m.Role == "Admin");

    if (!estAdmin)
        return Forbid();

    if (utilisateurId == userId)
        return BadRequest(new { message = "Vous ne pouvez pas vous retirer vous-même de cette façon. Utilisez 'Quitter le groupe'." });

    var membre = await _context.MembresGroupe
        .FirstOrDefaultAsync(m => m.GroupeId == id && m.UtilisateurId == utilisateurId);

    if (membre == null)
        return NotFound();

    _context.MembresGroupe.Remove(membre);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Membre retiré." });
}

// PUT /api/groupes/5/membres/3/promouvoir — admin seulement
[HttpPut("{id}/membres/{utilisateurId}/promouvoir")]
public async Task<IActionResult> PromouvoirMembre(int id, int utilisateurId)
{
    var userId = User.GetUserId();

    var estAdmin = await _context.MembresGroupe
        .AnyAsync(m => m.GroupeId == id && m.UtilisateurId == userId && m.Role == "Admin");

    if (!estAdmin)
        return Forbid();

    var membre = await _context.MembresGroupe
        .FirstOrDefaultAsync(m => m.GroupeId == id && m.UtilisateurId == utilisateurId);

    if (membre == null)
        return NotFound();

    membre.Role = membre.Role == "Admin" ? "Membre" : "Admin";
    await _context.SaveChangesAsync();

    return Ok(new { nouveauRole = membre.Role });
}
}