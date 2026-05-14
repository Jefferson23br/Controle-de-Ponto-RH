// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Oracle.EntityFrameworkCore.Infrastructure;

namespace ChronosPoint.Infrastructure.Persistence;

/// <summary>
/// Permite <c>dotnet ef</c> sem depender apenas do registo em runtime (AddDbContext so corre se houver connection string).
/// Carrega appsettings da API, User Secrets e variaveis de ambiente.
/// </summary>
public sealed class ChronosPointDbContextFactory : IDesignTimeDbContextFactory<ChronosPointDbContext>
{
    private const string ApiProjectFileName = "ChronosPoint.Api.csproj";
    private const string UserSecretsId = "chronospoint-api-9a8b7c6d-5e4f-3210-abcd-ef1234567890";

    /// <summary>
    /// So usada se definir <c>CHRONOSPOINT_EF_USE_PLACEHOLDER=1</c> no ambiente (ex.: gerar <c>migrations add</c> sem Oracle).
    /// Nunca use isto para <c>database update</c>: a actualizacao precisa de ligacao real ao servidor.
    /// </summary>
    private const string DesignTimePlaceholderOracle =
        "User Id=DESIGN_TIME;Password=DESIGN_TIME;Data Source=127.0.0.1:1521/XEPDB1;";

    public ChronosPointDbContext CreateDbContext(string[] args)
    {
        var apiDir = ResolveApiProjectDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiDir)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddUserSecrets(UserSecretsId, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var oracleConnection = OracleConnectionResolver.Resolve(configuration);
        if (string.IsNullOrWhiteSpace(oracleConnection))
        {
            var allowPlaceholder = IsTruthyEnvironmentFlag(Environment.GetEnvironmentVariable("CHRONOSPOINT_EF_USE_PLACEHOLDER"));

            if (allowPlaceholder)
            {
                oracleConnection = DesignTimePlaceholderOracle;
            }
            else
            {
                throw new InvalidOperationException(
                    "Oracle nao configurado para o EF Core (User Secrets na ChronosPoint.Api ou variaveis de ambiente). " +
                    "Defina ConnectionStrings:Oracle ou ORACLE_HOST, ORACLE_SERVICE_NAME, ORACLE_USER e ORACLE_PASSWORD. " +
                    "Comando para listar segredos: dotnet user-secrets list --project src/ChronosPoint.Api " +
                    "Para aplicar migracoes sem gravar segredos nesta maquina: dotnet ef database update ... --connection \"User Id=...;Password=...;Data Source=HOST:1521/SERVICE\". " +
                    "Se viu ORA-12541 ao correr database update, o cliente tentou ligar a um host sem listener (muitas vezes 127.0.0.1): confirme os segredos ou use --connection. " +
                    "Para gerar novas migracoes sem servidor Oracle: CHRONOSPOINT_EF_USE_PLACEHOLDER=1 (ver database/oracle/README.md).");
            }
        }

        var optionsBuilder = new DbContextOptionsBuilder<ChronosPointDbContext>();
        optionsBuilder.UseOracle(oracleConnection, o => o.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion21));
        return new ChronosPointDbContext(optionsBuilder.Options);
    }

    private static string ResolveApiProjectDirectory()
    {
        for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir != null; dir = dir.Parent)
        {
            var direct = Path.Combine(dir.FullName, ApiProjectFileName);
            if (File.Exists(direct))
            {
                return dir.FullName;
            }

            var nested = Path.Combine(dir.FullName, "ChronosPoint.Api", ApiProjectFileName);
            if (File.Exists(nested))
            {
                return Path.GetDirectoryName(nested)!;
            }
        }

        throw new InvalidOperationException(
            $"Nao foi possivel localizar {ApiProjectFileName}. Execute os comandos dotnet ef a partir da raiz da solucao ou da pasta src/ChronosPoint.Api.");
    }

    private static bool IsTruthyEnvironmentFlag(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return string.Equals(value.Trim(), "1", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value.Trim(), "true", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value.Trim(), "yes", StringComparison.OrdinalIgnoreCase);
    }
}
