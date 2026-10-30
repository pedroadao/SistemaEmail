using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SistemaEmail.Models;

namespace SistemaEmail.Services;

public class EmailSender(IConfiguration configuration)
{
    public async Task EnviarAsync(Email email, CancellationToken cancellationToken)
    {
        var mensagem = new MimeMessage();
        mensagem.From.Add(MailboxAddress.Parse(configuration["SMTP_USER"]!));

        AdicionarDestinatarios(mensagem.To, email.Destinatario);
        AdicionarDestinatarios(mensagem.Cc, email.Copia);
        AdicionarDestinatarios(mensagem.Bcc, email.CopiaOculta );

        mensagem.Subject = email.Assunto;
        mensagem.Body = new TextPart("plain")
        {
            Text = email.Corpo
        };

        using var cliente = new SmtpClient
        {
            CheckCertificateRevocation = false
        };
        var host = configuration["SMTP_HOST"]!;
        var porta = int.Parse(configuration["SMTP_PORT"] ?? "587");

        await cliente.ConnectAsync(host, porta, SecureSocketOptions.StartTls, cancellationToken);
        await cliente.AuthenticateAsync(configuration["SMTP_USER"]!, configuration["SMTP_PASSWORD"]!, cancellationToken);
        await cliente.SendAsync(mensagem, cancellationToken);
        await cliente.DisconnectAsync(true, cancellationToken);
    }

    private static void AdicionarDestinatarios(InternetAddressList lista, string? enderecos)
    {
        if (string.IsNullOrWhiteSpace(enderecos))
        {
            return;
        }

        foreach (var endereco in enderecos.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            lista.Add(MailboxAddress.Parse(endereco.Trim()));
        }
    }
}