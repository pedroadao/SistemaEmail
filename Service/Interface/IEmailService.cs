using SistemaEmail.Models;

namespace SistemaEmail.Services.Interface;

public interface IEmailService
{
    Task SendEmailAsync(Email email);
}