namespace ReseauUniversitaire.DTOs.Groupe;

public class GroupeDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool EstPrive { get; set; }
    public string? FiliereNom { get; set; }
    public int NbMembres { get; set; }
    public bool JeSuisMembre { get; set; }
}