using Microsoft.AspNetCore.Http;

namespace ReseauUniversitaire.Services;

public interface IFileService
{
    Task<string> UploadImageAsync(IFormFile fichier, string dossier);
    Task<string> UploadVideoAsync(IFormFile fichier, string dossier);
    Task<string> UploadFichierAsync(IFormFile fichier, string dossier);
}