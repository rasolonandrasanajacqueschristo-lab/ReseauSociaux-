using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Ressource;
using ReseauUniversitaire.Helpers;
using ReseauUniversitaire.Models;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RessourcesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RessourcesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/ressources?matiere=Algo&filiereId=1
    [HttpGet]
    public async Task<IActionResult> GetAll(string? matiere, int? filiereId)
    {
        var query = _context.Ressources
            .Include(r => r.Auteur)
            .Include(r => r.Filiere)
            .Include(r => r.Evaluations)
            .AsQueryable();

        if (!string.IsNullOrEmpty(matiere))
            query = query.Where(r => r.Matiere.Contains(matiere));

        if (filiereId.HasValue)
            query = query.Where(r => r.FiliereId == filiereId);

        var ressources = await query
            .OrderByDescending(r => r.DateUpload)
            .Select(r => new RessourceDto
            {
                Id = r.Id,
                Titre = r.Titre,
                Description = r.Description,
                FichierUrl = r.FichierUrl,
                TypeFichier = r.TypeFichier,
                Matiere = r.Matiere,
                NbTelechargements = r.NbTelechargements,
                DateUpload = r.DateUpload,
                AuteurId = r.AuteurId,
                AuteurNom = r.Auteur.Nom,
                AuteurPrenom = r.Auteur.Prenom,
                FiliereNom = r.Filiere == null ? null : r.Filiere.Nom,
                NoteMoyenne = r.Evaluations.Any() ? r.Evaluations.Average(e => e.Note) : 0,
                NbEvaluations = r.Evaluations.Count
            })
            .ToListAsync();

        return Ok(ressources);
    }

    // GET /api/ressources/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ressource = await _context.Ressources
            .Include(r => r.Auteur)
            .Include(r => r.Filiere)
            .Include(r => r.Evaluations)
            .Where(r => r.Id == id)
            .Select(r => new RessourceDto
            {
                Id = r.Id,
                Titre = r.Titre,
                Description = r.Description,
                FichierUrl = r.FichierUrl,
                TypeFichier = r.TypeFichier,
                Matiere = r.Matiere,
                NbTelechargements = r.NbTelechargements,
                DateUpload = r.DateUpload,
                AuteurId = r.AuteurId,
                AuteurNom = r.Auteur.Nom,
                AuteurPrenom = r.Auteur.Prenom,
                FiliereNom = r.Filiere == null ? null : r.Filiere.Nom,
                NoteMoyenne = r.Evaluations.Any() ? r.Evaluations.Average(e => e.Note) : 0,
                NbEvaluations = r.Evaluations.Count
            })
            .FirstOrDefaultAsync();

        if (ressource == null)
            return NotFound();

        return Ok(ressource);
    }

    // POST /api/ressources
    [HttpPost]
    public async Task<IActionResult> Create(CreateRessourceDto dto)
    {
        var userId = User.GetUserId();

        var ressource = new Ressource
        {
            Titre = dto.Titre,
            Description = dto.Description,
            FichierUrl = dto.FichierUrl,
            TypeFichier = dto.TypeFichier,
            Matiere = dto.Matiere,
            FiliereId = dto.FiliereId,
            AuteurId = userId
        };

        _context.Ressources.Add(ressource);
        await _context.SaveChangesAsync();

        return Ok(ressource);
    }

    // POST /api/ressources/5/telecharger
    [HttpPost("{id}/telecharger")]
    public async Task<IActionResult> Telecharger(int id)
    {
        var ressource = await _context.Ressources.FindAsync(id);
        if (ressource == null)
            return NotFound();

        ressource.NbTelechargements++;
        await _context.SaveChangesAsync();

        return Ok(new { fichierUrl = ressource.FichierUrl });
    }

    // POST /api/ressources/5/evaluer
    [HttpPost("{id}/evaluer")]
    public async Task<IActionResult> Evaluer(int id, EvaluerRessourceDto dto)
    {
        var userId = User.GetUserId();

        var ressource = await _context.Ressources.FindAsync(id);
        if (ressource == null)
            return NotFound();

        var evaluationExistante = await _context.EvaluationsRessource
            .FirstOrDefaultAsync(e => e.RessourceId == id && e.UtilisateurId == userId);

        if (evaluationExistante != null)
        {
            evaluationExistante.Note = dto.Note;
            evaluationExistante.Commentaire = dto.Commentaire;
        }
        else
        {
            _context.EvaluationsRessource.Add(new EvaluationRessource
            {
                RessourceId = id,
                UtilisateurId = userId,
                Note = dto.Note,
                Commentaire = dto.Commentaire
            });
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Évaluation enregistrée." });
    }

    // DELETE /api/ressources/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var ressource = await _context.Ressources.FindAsync(id);

        if (ressource == null)
            return NotFound();

        if (ressource.AuteurId != userId)
            return Forbid();

        _context.Ressources.Remove(ressource);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}