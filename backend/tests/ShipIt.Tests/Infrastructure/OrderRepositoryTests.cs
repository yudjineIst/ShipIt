using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ShipIt.Application.Common.Abstractions;
using ShipIt.Domain.Common;
using ShipIt.Domain.Orders;
using ShipIt.Domain.Orders.ValueObjects;

namespace ShipIt.Tests.Infrastructure;

public sealed class OrderRepositoryTests
{
    private static readonly DateOnly PickupDate = new(2026, 7, 10);

    [Fact]
    public async Task AddAsync_ShouldReturnConflict_WhenOrderNumberAlreadyExists()
    {
        using var factory = new TestApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var orderNumber = OrderNumber.Create("ORD-20260704-ABC123").Value;
        var firstOrder = CreateOrder(Guid.NewGuid(), orderNumber);
        var duplicateOrder = CreateOrder(Guid.NewGuid(), orderNumber);

        var firstResult = await repository.AddAsync(firstOrder, CancellationToken.None);
        var duplicateResult = await repository.AddAsync(duplicateOrder, CancellationToken.None);

        firstResult.IsSuccess.Should().BeTrue();
        duplicateResult.IsFailure.Should().BeTrue();
        duplicateResult.Error.Type.Should().Be(ErrorType.Conflict);
        duplicateResult.Error.Should().Be(OrderErrors.OrderNumberConflict);
    }

    private static DeliveryOrder CreateOrder(Guid id, OrderNumber orderNumber)
    {
        return DeliveryOrder.Create(
            id,
            orderNumber,
            "Москва",
            "Тверская улица",
            "10",
            5,
            "Тула",
            "Советская улица",
            "12",
            null,
            3.5m,
            PickupDate,
            new DateTime(2026, 7, 4, 10, 0, 0, DateTimeKind.Utc)).Value;
    }
}
