// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;

namespace ChronosPoint.Infrastructure;

/// <summary>
/// Resolve a connection string Oracle a partir de ConnectionStrings:Oracle
/// ou de variaveis ORACLE_* (uteis em User Secrets / ambiente sem expor uma unica string longa).
/// </summary>
public static class OracleConnectionResolver
{
    public static string? Resolve(IConfiguration configuration)
    {
        var configured = configuration.GetConnectionString("Oracle");
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured;
        }

        var host = configuration["ORACLE_HOST"];
        var port = configuration["ORACLE_PORT"] ?? "1521";
        var serviceName = configuration["ORACLE_SERVICE_NAME"];
        var user = configuration["ORACLE_USER"];
        var password = configuration["ORACLE_PASSWORD"];

        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(serviceName) ||
            string.IsNullOrWhiteSpace(user) ||
            password is null)
        {
            return null;
        }

        return $"User Id={user};Password={password};Data Source={host}:{port}/{serviceName}";
    }
}
