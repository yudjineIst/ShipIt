using Microsoft.EntityFrameworkCore;
using ShipIt.Infrastructure.Database;

namespace ShipIt.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShipItDbContext>();
        await dbContext.Database.MigrateAsync();
        await scope.ServiceProvider.GetRequiredService<DatabaseSeeder>().SeedAsync();
        app.Logger.LogInformation("Database migrations and demo seed completed");
    }
}
