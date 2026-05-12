// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

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

        var host = configuration["ORACLE_HOST"]?.Trim();
        var port = (configuration["ORACLE_PORT"] ?? "1521").Trim();
        var serviceName = configuration["ORACLE_SERVICE_NAME"]?.Trim();
        var user = configuration["ORACLE_USER"]?.Trim();
        var password = configuration["ORACLE_PASSWORD"]?.Trim();

        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(serviceName) ||
            string.IsNullOrWhiteSpace(user) ||
            string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        // Easy Connect (host:port/svc) + password com @ no fim pode confundir alguns caminhos; DESCRIPTION e mais explicito.
        var dataSource =
            $"(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={serviceName})))";

        var builder = new OracleConnectionStringBuilder
        {
            UserID = user,
            Password = password,
            DataSource = dataSource,
        };
        return builder.ConnectionString;
    }
}
