namespace ReseauUniversitaire.DTOs.Canal;

public class MessageCanalDto
{
    public int Id { get; set; }
    public string Contenu { get; set; } = string.Empty;
    public string? FichierUrl { get; set; }
    public DateTime DateEnvoi { get; set; }
    public int ExpediteurId { get; set; }
    public string ExpediteurNom { get; set; } = string.Empty;
    public string ExpediteurPrenom { get; set; } = string.Empty;
    public string? ExpediteurPhotoUrl { get; set; }
}

public class EnvoyerMessageCanalDto
{
    public string Contenu { get; set; } = string.Empty;
    public string? FichierUrl { get; set; }
}