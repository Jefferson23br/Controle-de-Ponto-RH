// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using ChronosPoint.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChronosPoint.Infrastructure.Persistence;

public class ChronosPointDbContext : DbContext
{
    public ChronosPointDbContext(DbContextOptions<ChronosPointDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserTenant> UserTenants => Set<UserTenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("CHRONOSPOINT_TENANTS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(256).IsRequired();
            entity.Property(e => e.CreatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("CHRONOSPOINT_USERS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(512).IsRequired();
            entity.Property(e => e.IsActive)
                  .HasConversion<int>()
                  .HasColumnType("NUMBER(1)")
                  .IsRequired();
            entity.Property(e => e.CreatedAtUtc).IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<UserTenant>(entity =>
        {
            entity.ToTable("CHRONOSPOINT_USER_TENANTS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Role)
                  .HasConversion<string>()
                  .HasMaxLength(20)
                  .IsRequired();
            entity.Property(e => e.CreatedAtUtc).IsRequired();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserTenants)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tenant)
                  .WithMany()
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.TenantId }).IsUnique();
        });
    }
}
