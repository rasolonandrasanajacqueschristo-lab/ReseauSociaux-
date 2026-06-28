namespace ReseauUniversitaire.DTOs.Message;

public class SendMessageDto
{
    public int DestinataireId { get; set; }
    public string Contenu { get; set; } = string.Empty;
    public string? FichierUrl { get; set; }
}