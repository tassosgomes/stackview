using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StackShare.Domain.Entities;

namespace StackShare.Infrastructure.Data;

public class StackShareDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public StackShareDbContext(DbContextOptions<StackShareDbContext> options) : base(options)
    {
    }

    public DbSet<Stack> Stacks { get; set; }
    public DbSet<Technology> Technologies { get; set; }
    public DbSet<StackTechnology> StackTechnologies { get; set; }
    public DbSet<StackHistory> StackHistories { get; set; }
    public DbSet<McpApiToken> McpApiTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Identity table names (opcional, para nomes mais limpos)
        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        // Configurar Stack
        builder.Entity<Stack>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Type).IsRequired().HasConversion<int>();
            entity.Property(e => e.IsPublic).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.HasIndex(e => new { e.UserId, e.Name }).IsUnique();

            // Relacionamento com User
            entity.HasOne(e => e.User)
                .WithMany(u => u.Stacks)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configurar Technology
        builder.Entity<Technology>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.IsPreRegistered).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configurar StackTechnology (many-to-many)
        builder.Entity<StackTechnology>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.StackId, e.TechnologyId }).IsUnique();

            entity.HasOne(e => e.Stack)
                .WithMany(s => s.StackTechnologies)
                .HasForeignKey(e => e.StackId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Technology)
                .WithMany(t => t.StackTechnologies)
                .HasForeignKey(e => e.TechnologyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configurar StackHistory
        builder.Entity<StackHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Type).IsRequired().HasConversion<int>();
            entity.Property(e => e.TechnologiesJson).IsRequired();
            entity.Property(e => e.Version).IsRequired();
            entity.HasIndex(e => new { e.StackId, e.Version }).IsUnique();

            // Relacionamento com Stack
            entity.HasOne(e => e.Stack)
                .WithMany(s => s.StackHistories)
                .HasForeignKey(e => e.StackId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento com User (ModifiedByUser)
            entity.HasOne(e => e.ModifiedByUser)
                .WithMany()
                .HasForeignKey(e => e.ModifiedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configurar McpApiToken
        builder.Entity<McpApiToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TokenHash).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsRevoked).IsRequired();
            entity.HasIndex(e => e.TokenHash);

            // Relacionamento com User
            entity.HasOne(e => e.User)
                .WithMany(u => u.McpApiTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}