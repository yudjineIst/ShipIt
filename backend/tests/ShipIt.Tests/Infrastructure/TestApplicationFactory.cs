using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ShipIt.Application.Common.Abstractions;
using ShipIt.Infrastructure.Database;

namespace ShipIt.Tests.Infrastructure;

public sealed class TestApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly DateTime DefaultUtcNow = new(2026, 7, 4, 10, 0, 0, DateTimeKind.Utc);
    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"shipit-tests-{Guid.NewGuid():N}.db");

    public TestDateTimeProvider DateTimeProvider { get; } = new(DefaultUtcNow);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Database", $"Data Source={_databasePath}");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDateTimeProvider>();
            services.AddSingleton<IDateTimeProvider>(DateTimeProvider);
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShipItDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(_databasePath))
                File.Delete(_databasePath);
        }
    }
}
