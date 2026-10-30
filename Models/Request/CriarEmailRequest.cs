using System.ComponentModel.DataAnnotations;

namespace SistemaEmail.Models.Request;

public class CriarEmailRequest
{
    [Required]
    public string Assunto { get; set; }
    [Required]
    public string Destinatario { get; set; }
    public string? Copia { get; set; }
    public string? CopiaOculta { get; set; }
    [Required]
    public string Corpo { get; set; }
}