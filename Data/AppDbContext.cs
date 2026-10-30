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

            entity.Property(e => e.Id   )
                .HasColumnName("id")
                .UseIdentityAlwaysColumn();

            entity.Property(e => e.Assunto)
                .HasColumnName("assunto")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Destinatario)
                .HasColumnName("destinatario")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Copia)
                .HasColumnName("copia")
                .HasMaxLength(200);

            entity.Property(e => e.CopiaOculta )
                .HasColumnName("copia_oculta")
                .HasMaxLength(200);

            entity.Property(e => e.Corpo)
                .HasColumnName("corpo")
                .HasMaxLength(8000)
                .IsRequired();

            entity.Property(e => e.CriadoEm)
                .HasColumnName("criado_em")
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.EnviadoEm)
                .HasColumnName("enviado_em")
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .IsRequired();

            entity.HasIndex(e => new { e.Status, e.CriadoEm });
        });
    }
}