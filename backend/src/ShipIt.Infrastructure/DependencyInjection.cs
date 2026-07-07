using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShipIt.Application.Common.Abstractions;
using ShipIt.Infrastructure.Database;
using ShipIt.Infrastructure.Time;

namespace ShipIt.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");

        services.AddDbContext<ShipItDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<DatabaseSeeder>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}
