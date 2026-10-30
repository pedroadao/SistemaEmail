using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEmail.Data;
using SistemaEmail.Models;
using SistemaEmail.Models.Request;
using SistemaEmail.Services.Interface;

namespace SistemaEmail.Controllers;


[ApiController]
[Route("api/emails")]
public class EmailController : ControllerBase
{
 private readonly AppDbContext _db;
 private readonly IEmailService _emailService;

    public EmailController(AppDbContext db, IEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }
    
    [HttpGet]
    public async Task<IActionResult>  Get()
    {
        var emails = await _db.Emails.ToListAsync();
        return Ok(emails);
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

        try
        {
            await _emailService.SendEmailAsync(email);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                mensagem = "Erro ao salvar o e-mail na fila.",
                detalhes = ex.Message
            });
        }

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