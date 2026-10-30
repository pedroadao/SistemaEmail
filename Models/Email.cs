namespace SistemaEmail.Models;

public class Email
{
    public long Id { get; set; }
    public string Assunto { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public string? Copia { get; set; }
    public string? CopiaOculta { get; set; }
    public string Corpo { get; set; } = string.Empty;
    public int Status { get; set; } = 1;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? EnviadoEm { get; set; }
}