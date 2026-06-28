using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Filiere;
using ReseauUniversitaire.Models;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FiliereController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FiliereController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/filiere — accessible sans authentification (page d'inscription)
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var filieres = await _context.Filieres
            .Include(f => f.Etudiants)
            .Select(f => new FiliereDto
            {
                Id = f.Id,
                Nom = f.Nom,
                Code = f.Code,
                Description = f.Description,
                NbEtudiants = f.Etudiants.Count
            })
            .ToListAsync();

        return Ok(filieres);
    }

    // GET /api/filiere/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var filiere = await _context.Filieres
            .Include(f => f.Etudiants)
            .Where(f => f.Id == id)
            .Select(f => new FiliereDto
            {
                Id = f.Id,
                Nom = f.Nom,
                Code = f.Code,
                Description = f.Description,
                NbEtudiants = f.Etudiants.Count
            })
            .FirstOrDefaultAsync();

        if (filiere == null)
            return NotFound();

        return Ok(filiere);
    }

    // POST /api/filiere — réservé Admin (on ajoutera la vérification de rôle plus tard)
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateFiliereDto dto)
    {
        var codeExiste = await _context.Filieres.AnyAsync(f => f.Code == dto.Code);
        if (codeExiste)
            return BadRequest(new { message = "Ce code de filière existe déjà." });

        var filiere = new Filiere
        {
            Nom = dto.Nom,
            Code = dto.Code,
            Description = dto.Description
        };

        _context.Filieres.Add(filiere);
        await _context.SaveChangesAsync();

        return Ok(filiere);
    }

    // DELETE /api/filiere/5 — réservé Admin
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var filiere = await _context.Filieres.FindAsync(id);
        if (filiere == null)
            return NotFound();

        _context.Filieres.Remove(filiere);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}