// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ChronosPoint.Infrastructure.Persistence;

/// <summary>
/// Permite <c>dotnet ef</c> sem depender apenas do registo em runtime (AddDbContext so corre se houver connection string).
/// Carrega appsettings da API, User Secrets e variaveis de ambiente.
/// </summary>
public sealed class ChronosPointDbContextFactory : IDesignTimeDbContextFactory<ChronosPointDbContext>
{
    private const string ApiProjectFileName = "ChronosPoint.Api.csproj";
    private const string UserSecretsId = "chronospoint-api-9a8b7c6d-5e4f-3210-abcd-ef1234567890";

    public ChronosPointDbContext CreateDbContext(string[] args)
    {
        var apiDir = ResolveApiProjectDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiDir)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddUserSecrets(UserSecretsId, optional: true)
            .AddEnvironmentVariables()
            .Build();

        var oracleConnection = OracleConnectionResolver.Resolve(configuration);
        if (string.IsNullOrWhiteSpace(oracleConnection))
        {
            throw new InvalidOperationException(
                "Defina ConnectionStrings:Oracle ou ORACLE_HOST, ORACLE_SERVICE_NAME, ORACLE_USER e ORACLE_PASSWORD " +
                "(User Secrets na ChronosPoint.Api, ou variaveis de ambiente / ConnectionStrings__Oracle).");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ChronosPointDbContext>();
        optionsBuilder.UseOracle(oracleConnection);
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
}
