// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using ChronosPoint.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChronosPoint.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var oracleConnection = OracleConnectionResolver.Resolve(configuration);
        if (string.IsNullOrWhiteSpace(oracleConnection))
        {
            return services;
        }

        services.AddDbContext<ChronosPointDbContext>(options =>
            options.UseOracle(oracleConnection));

        return services;
    }
}
