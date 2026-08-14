using Microsoft.AspNetCore.Authentication.Negotiate;
using Smbc.Risk.ReportingEngine.Infrastructure;
using Smbc.Risk.ReportingEngine.Application;
using Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Migrations;
using System.Runtime.CompilerServices;

namespace Smbc.Risk.ReportingEngine.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Services to container
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration);

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add authentication and authorization services
        builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            .AddNegotiate();
        builder.Services.AddAuthorization(options =>
        {
            // By default, all incoming requests will be authorized according to the default policy.
            options.FallbackPolicy = options.DefaultPolicy;
        });

        var app = builder.Build();

        // Make sure the Database is created
        await app.InitializeDatabaseAsync();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Enable authentication and authorization middleware
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
