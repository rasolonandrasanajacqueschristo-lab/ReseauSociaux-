namespace ReseauUniversitaire.DTOs.Filiere;

public class CreateFiliereDto
{
    public string Nom { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}