namespace ReseauUniversitaire.DTOs.Canal;

public class CanalDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool EstAdminSeulement { get; set; }
    public int GroupeId { get; set; }
}

public class CreateCanalDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool EstAdminSeulement { get; set; } = false;
}