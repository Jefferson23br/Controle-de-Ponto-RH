// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using ChronosPoint.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChronosPoint.Api;

/// <summary>
/// Rotas HTTP relacionadas com tenants (multi-empresa).
/// Classe estatica: agrupa metodos de extensao sem estado; padrao comum em Minimal APIs.
/// </summary>
public static class TenantsEndpoints
{
    /// <summary>
    /// Regista os endpoints nesta aplicacao (<see cref="WebApplication"/>).
    /// </summary>
    public static WebApplication MapTenantsApi(this WebApplication app)
    {
        // MapGet associa um URL a uma funcao. O ASP.NET Core injeta HttpContext e CancellationToken automaticamente.
        app.MapGet("/api/tenants", ListTenantsAsync);
        return app;
    }

    /// <summary>
    /// Lista tenants sem rastrear alteracoes (AsNoTracking: leitura mais leve).
    /// </summary>
    private static async Task<IResult> ListTenantsAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        // GetService devolve null se AddDbContext nao correu (Oracle nao configurado em DependencyInjection).
        var db = httpContext.RequestServices.GetService<ChronosPointDbContext>();
        if (db is null)
        {
            return Results.Json(
                new { detail = "Base de dados nao configurada. Defina ConnectionStrings:Oracle ou variaveis ORACLE_*." },
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        var items = await db.Tenants
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new { t.Id, t.Name, t.CreatedAtUtc })
            .ToListAsync(cancellationToken);

        return Results.Ok(items);
    }
}
