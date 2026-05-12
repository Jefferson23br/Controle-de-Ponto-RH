// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using ChronosPoint.Infrastructure;
using ChronosPoint.Infrastructure.Persistence;
using ChronosPoint.Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                  ?? Array.Empty<string>();
if (corsOrigins.Length > 0)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Frontend", policy =>
        {
            policy.WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
if (corsOrigins.Length > 0)
{
    app.UseCors("Frontend");
}

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    service = "ChronosPoint.Api",
    timeUtc = DateTimeOffset.UtcNow
}));

app.MapGet("/api/health/database", async (IServiceProvider sp, CancellationToken ct) =>
{
    await using var scope = sp.CreateAsyncScope();
    var db = scope.ServiceProvider.GetService<ChronosPointDbContext>();
    if (db is null)
    {
        return Results.Json(new { status = "skipped", detail = "ConnectionStrings:Oracle nao configurada." });
    }

    var canConnect = await db.Database.CanConnectAsync(ct);
    return canConnect
        ? Results.Ok(new { status = "ok", database = "oracle" })
        : Results.Problem("Nao foi possivel ligar ao Oracle.", statusCode: 503);
});

// Rotas de negocio agrupadas em extensoes (ficheiros separados) para o Program.cs ficar legivel.
app.MapTenantsApi();

app.Run();
