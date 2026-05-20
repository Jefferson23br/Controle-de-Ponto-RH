// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using ChronosPoint.Application.Auth;

namespace ChronosPoint.Api;

public static class AuthEndpoints
{
    public static WebApplication MapAuthApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", RegisterAsync);
        group.MapPost("/login", LoginAsync);
        group.MapGet("/me", GetCurrentUserAsync).RequireAuthorization();

        return app;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        IAuthService auth,
        CancellationToken ct)
    {
        var result = await auth.RegisterAsync(request, ct);

        if (!result.Success)
            return Results.BadRequest(new { error = result.Error });

        return Results.Created($"/api/users/{result.Token!.User.Id}", result.Token);
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        IAuthService auth,
        CancellationToken ct)
    {
        var result = await auth.LoginAsync(request, ct);

        if (!result.Success)
            return Results.UnprocessableEntity(new { error = result.Error });

        if (result.TenantChoice is not null)
            return Results.Ok(new
            {
                requiresTenantSelection = true,
                user = result.TenantChoice.User,
                tenants = result.TenantChoice.Tenants
            });

        return Results.Ok(result.Token);
    }

    private static IResult GetCurrentUserAsync(HttpContext httpContext)
    {
        var user = httpContext.User;
        var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                  ?? user.FindFirst("sub")?.Value;
        var email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                 ?? user.FindFirst("email")?.Value;
        var fullName = user.FindFirst("fullName")?.Value;
        var tenantId = user.FindFirst("tenantId")?.Value;
        var tenantName = user.FindFirst("tenantName")?.Value;
        var role = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        return Results.Ok(new
        {
            userId,
            email,
            fullName,
            tenantId,
            tenantName,
            role
        });
    }
}
