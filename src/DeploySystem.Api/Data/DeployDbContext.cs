using DeploySystem.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeploySystem.Api.Data;

public class DeployDbContext : DbContext
{
    public DeployDbContext(DbContextOptions<DeployDbContext> options) : base(options) { }

    public DbSet<Imagem> Imagens => Set<Imagem>();
    public DbSet<Perfil> Perfis => Set<Perfil>();
    public DbSet<Pacote> Pacotes => Set<Pacote>();
    public DbSet<PerfilPacote> PerfilPacotes => Set<PerfilPacote>();
    public DbSet<Deploy> Deploys => Set<Deploy>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Imagem
        mb.Entity<Imagem>(e =>
        {
            e.ToTable("Imagens");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            e.Property(x => x.Versao).HasMaxLength(50);
            e.Property(x => x.CaminhoWim).HasMaxLength(500).IsRequired();
            e.Property(x => x.Hash).HasMaxLength(100);
        });

        // Perfil
        mb.Entity<Perfil>(e =>
        {
            e.ToTable("Perfis");
            e.HasKey(x => x.Id);
            e.Property(x => x.Codigo).HasMaxLength(20).IsRequired();
            e.HasIndex(x => x.Codigo).IsUnique();
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            e.Property(x => x.Descricao).HasMaxLength(500);

            e.HasOne(x => x.Imagem)
             .WithMany(i => i.Perfis)
             .HasForeignKey(x => x.ImagemId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Pacote
        mb.Entity<Pacote>(e =>
        {
            e.ToTable("Pacotes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            e.Property(x => x.Tipo).HasMaxLength(20).IsRequired();
            e.Property(x => x.Caminho).HasMaxLength(500).IsRequired();
            e.Property(x => x.Argumentos).HasMaxLength(500);
            e.Property(x => x.Hash).HasMaxLength(100);
        });

        // PerfilPacote (N:N com ordem)
        mb.Entity<PerfilPacote>(e =>
        {
            e.ToTable("PerfilPacotes");
            e.HasKey(x => new { x.PerfilId, x.PacoteId });

            e.HasOne(x => x.Perfil)
             .WithMany(p => p.PerfilPacotes)
             .HasForeignKey(x => x.PerfilId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Pacote)
             .WithMany(p => p.PerfilPacotes)
             .HasForeignKey(x => x.PacoteId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Deploy
        mb.Entity<Deploy>(e =>
        {
            e.ToTable("Deploys");
            e.HasKey(x => x.Id);
            e.Property(x => x.CodigoPerfil).HasMaxLength(20).IsRequired();
            e.Property(x => x.Hostname).HasMaxLength(100).IsRequired();
            e.Property(x => x.MacAddress).HasMaxLength(50);
            e.Property(x => x.Status).HasMaxLength(30).IsRequired();
            e.Property(x => x.PacoteAtual).HasMaxLength(100);
            e.Property(x => x.MensagemErro).HasMaxLength(1000);

            e.HasIndex(x => x.Hostname);
            e.HasIndex(x => x.Status);
        });

        // Usuario
        mb.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            e.Property(x => x.Login).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Login).IsUnique();
            e.Property(x => x.SenhaHash).HasMaxLength(200).IsRequired();
            e.Property(x => x.Perfil).HasMaxLength(20).IsRequired();
        });
    }
}
