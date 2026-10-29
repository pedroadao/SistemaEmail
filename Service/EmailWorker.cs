using Microsoft.EntityFrameworkCore;
using SistemaEmail.Data;

namespace SistemaEmail.Services;

public class EmailWorker(IServiceScopeFactory scopeFactory, ILogger<EmailWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessarEmailsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao consultar a fila de e-mails.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task ProcessarEmailsAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sender = scope.ServiceProvider.GetRequiredService<EmailSender>();
        var emails = await db.Emails
            .Where(e => e.Status == "Pendente")
            .OrderBy(e => e.CriadoEm)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var email in emails)
        {
            try
            {
                await sender.EnviarAsync(email, cancellationToken);
                email.Status = "Enviado";
                email.EnviadoEm = DateTime.UtcNow;
                email.Erro = null;

                logger.LogInformation("E-mail {EmailId} enviado.", email.Id);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                email.Status = "Falhou";
                email.Erro = ex.Message;

                logger.LogError(ex, "Falha ao enviar o e-mail {EmailId}.", email.Id);
            }

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}