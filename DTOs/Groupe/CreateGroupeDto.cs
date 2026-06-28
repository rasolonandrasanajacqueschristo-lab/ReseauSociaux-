namespace ReseauUniversitaire.DTOs.Groupe;

public class CreateGroupeDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool EstPrive { get; set; } = false;
    public int? FiliereId { get; set; }
}