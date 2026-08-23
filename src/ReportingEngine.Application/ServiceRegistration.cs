using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smbc.Risk.Core.Application.Services;
using Smbc.Risk.ReportingEngine.Application.Services;
using Smbc.Risk.ReportingEngine.Domain.Services;

namespace Smbc.Risk.ReportingEngine.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuraion)
    {
        // Configure Auto Mapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Make sure Mapping Profiles are all valid 
        var mapperConfiguraion = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
        });
        mapperConfiguraion.AssertConfigurationIsValid();


        // Configure all service dependencies
        services.AddScoped<ISystemParameterTypeService, SystemParameterTypeService>();
        services.AddScoped<IExcelParserService, ExcelParserService>();
        services.AddScoped<IReportManagementService, ReportManagementService>();

        return services;
    }
}
