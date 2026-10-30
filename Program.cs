using Microsoft.EntityFrameworkCore;
using SistemaEmail.Data;
using SistemaEmail.Services;
using SistemaEmail.services;
using SistemaEmail.Services.Interface;

LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string não configurada.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<EmailSender>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<EmailWorker>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

// No Docker, as variáveis do .env já chegam via env_file; localmente (dotnet run)
// o ASP.NET Core não lê .env, então carregamos aqui sem sobrescrever o que já existir.
static void LoadDotEnv()
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (!File.Exists(path))
    {
        return;
    }

    foreach (var linha in File.ReadAllLines(path))
    {
        var texto = linha.Trim();
        if (texto.Length == 0 || texto.StartsWith('#'))
        {
            continue;
        }

        var separador = texto.IndexOf('=');
        if (separador <= 0)
        {
            continue;
        }

        var chave = texto[..separador].Trim();
        var valor = texto[(separador + 1)..].Trim().Trim('"');

        if (Environment.GetEnvironmentVariable(chave) is null)
        {
            Environment.SetEnvironmentVariable(chave, valor);
        }
    }
}