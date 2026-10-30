using SistemaEmail.Data;
using SistemaEmail.Models;
using SistemaEmail.Services.Interface;

namespace SistemaEmail.services;

public class EmailService : IEmailService
{
    private readonly AppDbContext _db;

    public EmailService(AppDbContext db)
    {
        _db = db;
    }

    public async Task SendEmailAsync(Email email)
    {
         try 
        { 
            _db.Emails.Add(email);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao salvar o e-mail na fila.", ex);
        }
    }
}