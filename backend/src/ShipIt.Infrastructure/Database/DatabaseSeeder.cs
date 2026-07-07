using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipIt.Application.Common.Abstractions;
using ShipIt.Application.Features;
using ShipIt.Domain.Orders;

namespace ShipIt.Infrastructure.Database;

public sealed class DatabaseSeeder(
    ShipItDbContext dbContext,
    IOrderNumberGenerator orderNumberGenerator,
    IDateTimeProvider dateTimeProvider,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await dbContext.DeliveryOrders.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Demo order seed skipped because orders already exist");
            return;
        }

        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow);
        var samples = new[]
        {
            new DemoOrder("Москва", "Ленинградский проспект", "36", 12,
                "Санкт-Петербург", "Невский проспект", "28", 7, 12.5m, 2),
            new DemoOrder("Казань", "улица Баумана", "15", null,
                "Самара", "Московское шоссе", "17", 24, 4.75m, 3),
            new DemoOrder("Екатеринбург", "улица Малышева", "51", 8,
                "Тюмень", "улица Республики", "61", null, 28.2m, 5),
            new DemoOrder("Новосибирск", "Красный проспект", "39", 16,
                "Омск", "улица Ленина", "20", 5, 7.4m, 4)
        };

        for (var index = 0; index < samples.Length; index++)
        {
            var sample = samples[index];
            var order = DeliveryOrder.Create(
                Guid.NewGuid(),
                orderNumberGenerator.Generate(),
                sample.SenderCity,
                sample.SenderStreet,
                sample.SenderHouse,
                sample.SenderApartment,
                sample.RecipientCity,
                sample.RecipientStreet,
                sample.RecipientHouse,
                sample.RecipientApartment,
                sample.CargoWeight,
                today.AddDays(sample.PickupAfterDays),
                dateTimeProvider.UtcNow.AddMinutes(-index));

            if (order.IsFailure)
            {
                logger.LogError("Demo order creation failed: {ErrorCode}", order.Error.Code);
                throw new InvalidOperationException(order.Error.Message);
            }

            dbContext.DeliveryOrders.Add(order.Value);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {OrderCount} demo orders", samples.Length);
    }

    private sealed record DemoOrder(
        string SenderCity,
        string SenderStreet,
        string SenderHouse,
        int? SenderApartment,
        string RecipientCity,
        string RecipientStreet,
        string RecipientHouse,
        int? RecipientApartment,
        decimal CargoWeight,
        int PickupAfterDays);
}
