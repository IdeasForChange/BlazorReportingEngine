using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smbc.ReportingEngine.Domain.Shared.Enums;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework;
using Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;

namespace Smbc.Risk.ReportingEngine.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string provider = configuration["DatabaseProvider"] ?? DatabaseProvider.SqlServer.ToString();
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found!");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            _ = provider.ToLowerInvariant() switch
            {
                "sqlserver" or "mssql" => options.UseSqlServer(connectionString, x => x.UseNodaTime()),
                "postgres" or "postgresql" => options.UseNpgsql(connectionString),
                "sqlite" => options.UseSqlite(connectionString),
                "oracle" => options.UseOracle(connectionString),
                _ => throw new InvalidOperationException($"Unsupported Database Provider: '{provider}'")
            };
        });

        // Configure BASE dependencies
        services.AddScoped<ISystemParameterTypeRepository, SystemParameterTypeRepository>();

        return services;
    }
}
