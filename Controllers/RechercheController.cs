using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RechercheController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RechercheController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/recherche?q=algo
    [HttpGet]
    public async Task<IActionResult> Rechercher(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(new { utilisateurs = Array.Empty<object>(), groupes = Array.Empty<object>(), ressources = Array.Empty<object>() });

        var utilisateurs = await _context.Utilisateurs
            .Where(u => u.Nom.Contains(q) || u.Prenom.Contains(q) || u.Email.Contains(q))
            .Select(u => new { u.Id, u.Nom, u.Prenom, u.PhotoUrl })
            .Take(10)
            .ToListAsync();

        var groupes = await _context.Groupes
            .Where(g => g.Nom.Contains(q))
            .Select(g => new { g.Id, g.Nom, g.Description })
            .Take(10)
            .ToListAsync();

        var ressources = await _context.Ressources
            .Where(r => r.Titre.Contains(q) || r.Matiere.Contains(q))
            .Select(r => new { r.Id, r.Titre, r.Matiere })
            .Take(10)
            .ToListAsync();

        return Ok(new { utilisateurs, groupes, ressources });
    }
}