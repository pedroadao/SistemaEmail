using Microsoft.AspNetCore.Mvc;
using SistemaEmail.Data;
using SistemaEmail.Models;

namespace SistemaEmail.Controllers;

public record CriarEmailRequest(
    string Assunto,
    string Destinatario,
    string? Copia,
    string? CopiaOculta,
    string Corpo
);

[ApiController]
[Route("api/emails")]
public class EmailController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(DateTime.Now);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarEmailRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Assunto) || string.IsNullOrWhiteSpace(request.Destinatario) || string.IsNullOrWhiteSpace(request.Corpo))
        {
            return BadRequest(new
            {
                mensagem = "Assunto, destinatário e corpo são obrigatórios."
            });
        }

        var email = new Email
        {
            Assunto = request.Assunto,
            Destinatario = request.Destinatario,
            Copia = request.Copia,
            CopiaOculta = request.CopiaOculta,
            Corpo = request.Corpo,
            Status = 1,
            CriadoEm = DateTime.UtcNow
        };

        db.Emails.Add(email);

        await db.SaveChangesAsync(cancellationToken);

        return Created(
            $"/api/emails/{email.Id}",
            new
            {
                email.Id,
                email.Status,
                mensagem = "E-mail adicionado à fila."
            });
    }
}