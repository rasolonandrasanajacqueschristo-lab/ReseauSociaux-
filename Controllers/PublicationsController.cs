using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReseauUniversitaire.Data;
using ReseauUniversitaire.DTOs.Publication;
using ReseauUniversitaire.Helpers;
using ReseauUniversitaire.Models;
using ReseauUniversitaire.Services;

namespace ReseauUniversitaire.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PublicationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public PublicationsController(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var userId = User.GetUserId();

        var publications = await _context.Publications
            .Include(p => p.Auteur)
            .Include(p => p.Likes)
            .Include(p => p.Commentaires)
            .Where(p => !p.EstSignale)
            .OrderByDescending(p => p.DateCreation)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PublicationDto
            {
                Id = p.Id,
                Contenu = p.Contenu,
                ImageUrl = p.ImageUrl,
                VideoUrl = p.VideoUrl,
                FichierUrl = p.FichierUrl,
                FichierNom = p.FichierNom,
                Tags = p.Tags,
                DateCreation = p.DateCreation,
                AuteurId = p.AuteurId,
                AuteurNom = p.Auteur.Nom,
                AuteurPrenom = p.Auteur.Prenom,
                AuteurPhotoUrl = p.Auteur.PhotoUrl,
                NbLikes = p.Likes.Count,
                NbCommentaires = p.Commentaires.Count,
                EstLikeParMoi = p.Likes.Any(l => l.UtilisateurId == userId)
            })
            .ToListAsync();

        return Ok(publications);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();

        var publication = await _context.Publications
            .Include(p => p.Auteur)
            .Include(p => p.Likes)
            .Include(p => p.Commentaires)
            .Where(p => p.Id == id)
            .Select(p => new PublicationDto
            {
                Id = p.Id,
                Contenu = p.Contenu,
                ImageUrl = p.ImageUrl,
                VideoUrl = p.VideoUrl,
                FichierUrl = p.FichierUrl,
                FichierNom = p.FichierNom,
                Tags = p.Tags,
                DateCreation = p.DateCreation,
                AuteurId = p.AuteurId,
                AuteurNom = p.Auteur.Nom,
                AuteurPrenom = p.Auteur.Prenom,
                AuteurPhotoUrl = p.Auteur.PhotoUrl,
                NbLikes = p.Likes.Count,
                NbCommentaires = p.Commentaires.Count,
                EstLikeParMoi = p.Likes.Any(l => l.UtilisateurId == userId)
            })
            .FirstOrDefaultAsync();

        if (publication == null)
            return NotFound();

        return Ok(publication);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePublicationDto dto)
    {
        var userId = User.GetUserId();

        var publication = new Publication
        {
            Contenu = dto.Contenu,
            ImageUrl = dto.ImageUrl,
            VideoUrl = dto.VideoUrl,
            FichierUrl = dto.FichierUrl,
            FichierNom = dto.FichierNom,
            Tags = dto.Tags,
            GroupeId = dto.GroupeId,
            AuteurId = userId
        };

        _context.Publications.Add(publication);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = publication.Id }, publication);
    }

    // POST /api/publications/upload-media — upload image OU vidéo OU fichier
    [HttpPost("upload-media")]
    public async Task<IActionResult> UploadMedia(IFormFile fichier, [FromQuery] string type)
    {
        if (fichier == null || fichier.Length == 0)
            return BadRequest(new { message = "Aucun fichier fourni." });

        const long maxImage = 10 * 1024 * 1024;   // 10 Mo
        const long maxVideo = 100 * 1024 * 1024;  // 100 Mo
        const long maxFichier = 25 * 1024 * 1024; // 25 Mo

        try
        {
            string url;
            switch (type)
            {
                case "image":
                    if (fichier.Length > maxImage)
                        return BadRequest(new { message = "Image trop volumineuse (max 10 Mo)." });
                    url = await _fileService.UploadImageAsync(fichier, "publications/images");
                    return Ok(new { url, type = "image" });

                case "video":
                    if (fichier.Length > maxVideo)
                        return BadRequest(new { message = "Vidéo trop volumineuse (max 100 Mo)." });
                    url = await _fileService.UploadVideoAsync(fichier, "publications/videos");
                    return Ok(new { url, type = "video" });

                case "fichier":
                    if (fichier.Length > maxFichier)
                        return BadRequest(new { message = "Fichier trop volumineux (max 25 Mo)." });
                    url = await _fileService.UploadFichierAsync(fichier, "publications/fichiers");
                    return Ok(new { url, type = "fichier", nom = fichier.FileName });

                default:
                    return BadRequest(new { message = "Type de média invalide." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Erreur upload: {ex.Message}" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var publication = await _context.Publications.FindAsync(id);

        if (publication == null)
            return NotFound();

        if (publication.AuteurId != userId)
            return Forbid();

        _context.Publications.Remove(publication);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/like")]
    public async Task<IActionResult> ToggleLike(int id)
    {
        var userId = User.GetUserId();

        var publication = await _context.Publications.FindAsync(id);
        if (publication == null)
            return NotFound();

        var likeExistant = await _context.Likes
            .FirstOrDefaultAsync(l => l.PublicationId == id && l.UtilisateurId == userId);

        if (likeExistant != null)
        {
            _context.Likes.Remove(likeExistant);
            await _context.SaveChangesAsync();
            return Ok(new { liked = false });
        }

        _context.Likes.Add(new Like
        {
            PublicationId = id,
            UtilisateurId = userId
        });

        if (publication.AuteurId != userId)
        {
            var liker = await _context.Utilisateurs.FindAsync(userId);
            _context.Notifications.Add(new Notification
            {
                UtilisateurId = publication.AuteurId,
                Message = $"{liker!.Prenom} a aimé votre publication",
                Type = "Like",
                LienUrl = $"/publications/{id}"
            });
        }

        await _context.SaveChangesAsync();

        return Ok(new { liked = true });
    }
}