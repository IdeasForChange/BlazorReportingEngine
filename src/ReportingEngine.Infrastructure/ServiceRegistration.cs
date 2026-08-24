using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smbc.Risk.ReportingEngine.Application.Interfaces;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;
using Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework;
using Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;
using Smbc.Risk.ReportingEngine.Infrastructure.Services;

namespace Smbc.Risk.ReportingEngine.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string provider = configuration["DatabaseProvider"] ?? DatabaseType.SqlServer.ToString();
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
        services.AddTransient<IDynamicQueryExecutor, DynamicQueryExecutor>();
        services.AddTransient<IReportParameterRepository, ReportParameterRepository>();
        services.AddTransient<IReportMetricRepository, ReportMetricRepository>();
        services.AddTransient<IReportTemplateRepository, ReportTemplateRepository>();
        services.AddTransient<IReportMasterRepository, ReportMasterRepository>();
        services.AddTransient<IDatabaseConnectionRepository, DatabaseConnectionRepository>();


        // 4. Background Multi-Threaded Execution Engine
        //services.AddHostedService<ReportRunnerWorker>();

        return services;
    }
}
