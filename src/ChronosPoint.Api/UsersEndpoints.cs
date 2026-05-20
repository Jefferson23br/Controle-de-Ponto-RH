// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using ChronosPoint.Application.Users;
using ChronosPoint.Domain.Entities;
using ChronosPoint.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChronosPoint.Api;

public static class UsersEndpoints
{
    public static WebApplication MapUsersApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        group.MapGet("/", ListUsersAsync);
        group.MapGet("/{id:guid}", GetUserByIdAsync);
        group.MapPost("/", CreateUserAsync);
        group.MapPut("/{id:guid}/toggle-active", ToggleActiveAsync);

        return app;
    }

    private static async Task<IResult> ListUsersAsync(
        HttpContext httpContext,
        ChronosPointDbContext db,
        CancellationToken ct)
    {
        var tenantId = GetCurrentTenantId(httpContext);
        if (tenantId is null)
            return Results.Forbid();

        var users = await db.UserTenants
            .AsNoTracking()
            .Where(ut => ut.TenantId == tenantId.Value)
            .Include(ut => ut.User)
            .Select(ut => new
            {
                ut.User.Id,
                ut.User.FullName,
                ut.User.Email,
                ut.User.IsActive,
                ut.User.CreatedAtUtc,
                ut.Role
            })
            .OrderBy(u => u.FullName)
            .ToListAsync(ct);

        return Results.Ok(users);
    }

    private static async Task<IResult> GetUserByIdAsync(
        Guid id,
        ChronosPointDbContext db,
        CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.UserTenants)
                .ThenInclude(ut => ut.Tenant)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user is null)
            return Results.NotFound(new { error = "Utilizador nao encontrado." });

        var response = new UserResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.IsActive,
            user.CreatedAtUtc,
            user.UserTenants.Select(ut => new UserTenantResponse(
                ut.TenantId, ut.Tenant.Name, ut.Role)).ToList());

        return Results.Ok(response);
    }

    private static async Task<IResult> CreateUserAsync(
        CreateUserRequest request,
        HttpContext httpContext,
        ChronosPointDbContext db,
        CancellationToken ct)
    {
        var currentRole = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (currentRole != nameof(TenantRole.Admin))
            return Results.Forbid();

        var emailNormalized = request.Email.Trim().ToUpperInvariant();
        var existingUser = await db.Users
            .FirstOrDefaultAsync(u => u.Email == emailNormalized, ct);

        var tenant = await db.Tenants.FindAsync(new object[] { request.TenantId }, ct);
        if (tenant is null)
            return Results.BadRequest(new { error = "Empresa nao encontrada." });

        if (existingUser is not null)
        {
            var alreadyLinked = await db.UserTenants
                .AnyAsync(ut => ut.UserId == existingUser.Id && ut.TenantId == request.TenantId, ct);
            if (alreadyLinked)
                return Results.Conflict(new { error = "Utilizador ja pertence a esta empresa." });

            db.UserTenants.Add(new UserTenant
            {
                Id = Guid.NewGuid(),
                UserId = existingUser.Id,
                TenantId = request.TenantId,
                Role = request.Role,
                CreatedAtUtc = DateTimeOffset.UtcNow
            });
            await db.SaveChangesAsync(ct);

            return Results.Ok(new { message = "Utilizador existente vinculado a empresa.", userId = existingUser.Id });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = emailNormalized,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        user.UserTenants.Add(new UserTenant
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TenantId = request.TenantId,
            Role = request.Role,
            CreatedAtUtc = DateTimeOffset.UtcNow
        });

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/api/users/{user.Id}", new { user.Id, user.FullName, user.Email });
    }

    private static async Task<IResult> ToggleActiveAsync(
        Guid id,
        HttpContext httpContext,
        ChronosPointDbContext db,
        CancellationToken ct)
    {
        var currentRole = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (currentRole != nameof(TenantRole.Admin))
            return Results.Forbid();

        var user = await db.Users.FindAsync(new object[] { id }, ct);
        if (user is null)
            return Results.NotFound(new { error = "Utilizador nao encontrado." });

        user.IsActive = !user.IsActive;
        await db.SaveChangesAsync(ct);

        return Results.Ok(new { user.Id, user.IsActive });
    }

    private static Guid? GetCurrentTenantId(HttpContext httpContext)
    {
        var tenantClaim = httpContext.User.FindFirst("tenantId")?.Value;
        return Guid.TryParse(tenantClaim, out var id) ? id : null;
    }
}
