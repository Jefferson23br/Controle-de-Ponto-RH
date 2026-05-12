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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("CHRONOSPOINT_TENANTS");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(256).IsRequired();
            entity.Property(e => e.CreatedAtUtc).IsRequired();
        });
    }
}
