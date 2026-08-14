using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Migrations;

public static class SQLiteMigration
{
    public static async Task InitializeDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            // Ensures the SQLite database file exists and all migrations are applied
            logger.LogInformation("Applying migrations for SQLite database...");
            await context.Database.MigrateAsync();
            logger.LogInformation("SQLite Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the SQLite database.");
            throw;
        }
    }
}
