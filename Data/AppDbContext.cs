using Microsoft.EntityFrameworkCore;
using SistemaEmail.Models;

namespace SistemaEmail.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Email> Emails => Set<Email>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Email>(entity =>
        {
            entity.ToTable("mail");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn();

            entity.Property(e => e.Assunto)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Destinatario)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(e => e.Copia)
                .HasMaxLength(1000);

            entity.Property(e => e.CopiaOculta)
                .HasMaxLength(1000);

            entity.Property(e => e.Corpo)
                .HasColumnType("text")
                .IsRequired();

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.CriadoEm)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.EnviadoEm)
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.Erro)
                .HasColumnType("text");

            entity.HasIndex(e => new { e.Status, e.CriadoEm });
        });
    }
}