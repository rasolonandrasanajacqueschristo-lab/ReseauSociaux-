using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace ReseauUniversitaire.Services;

public class FileService : IFileService
{
    private readonly Cloudinary _cloudinary;

    public FileService(IConfiguration config)
    {
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile fichier, string dossier)
    {
        using var stream = fichier.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fichier.FileName, stream),
            Folder = dossier,
            Transformation = new Transformation().Width(1200).Height(1200).Crop("limit")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result.SecureUrl.ToString();
    }

    public async Task<string> UploadVideoAsync(IFormFile fichier, string dossier)
    {
        using var stream = fichier.OpenReadStream();

        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(fichier.FileName, stream),
            Folder = dossier,
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result.SecureUrl.ToString();
    }

    public async Task<string> UploadFichierAsync(IFormFile fichier, string dossier)
    {
        using var stream = fichier.OpenReadStream();

        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(fichier.FileName, stream),
            Folder = dossier,
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result.SecureUrl.ToString();
    }
}