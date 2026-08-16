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

        // 1. Configure permissive CORS policy for intranet access
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorApp", policy =>
            {
                // Wildcard allow for any header and HTTP method (GET, POST, PUT, DELETE, etc.)
                policy.AllowAnyHeader()
                      .AllowAnyMethod()
                      // In production/intranet, specify the Blazor app URL or use AllowAnyOrigin()
                      .SetIsOriginAllowed(_ => true); // Allows connections from any origin
            });
        });

        // Add Services to container
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration);

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //// Add authentication and authorization services
        //builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
        //    .AddNegotiate();
        //builder.Services.AddAuthorization(options =>
        //{
        //    // By default, all incoming requests will be authorized according to the default policy.
        //    options.FallbackPolicy = options.DefaultPolicy;
        //});

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // 2. Enable CORS - MUST be called after UseRouting and before UseAuthorization
        app.UseRouting();
        app.UseCors("AllowBlazorApp");

        // Make sure the Database is created
        // await app.InitializeDatabaseAsync();
        app.UseHttpsRedirection();

        // 3. Ensure endpoints default to anonymous
        app.UseAuthorization();
        app.MapControllers().AllowAnonymous();

        app.Run();
    }
}
