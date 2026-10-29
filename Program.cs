using Microsoft.EntityFrameworkCore;
using SistemaEmail.Data;
using SistemaEmail.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string não configurada.");

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<EmailSender>();
builder.Services.AddHostedService<EmailWorker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await db.Database.EnsureCreatedAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();