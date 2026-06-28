namespace ReseauUniversitaire.DTOs.Filiere;

public class FiliereDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int NbEtudiants { get; set; }
}