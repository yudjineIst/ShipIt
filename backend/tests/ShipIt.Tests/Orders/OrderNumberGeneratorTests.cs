using FluentAssertions;
using ShipIt.Application.Features;
using ShipIt.Tests.Infrastructure;

namespace ShipIt.Tests.Orders;

public sealed class OrderNumberGeneratorTests
{
    [Fact]
    public void OrderNumberGenerator_ShouldCreateHumanReadableNumber()
    {
        var dateTimeProvider = new TestDateTimeProvider(
            new DateTime(2026, 7, 4, 12, 30, 0, DateTimeKind.Utc));
        var generator = new OrderNumberGenerator(dateTimeProvider);

        var orderNumber = generator.Generate();

        orderNumber.Value.Should().MatchRegex("^ORD-20260704-[A-F0-9]{6}$");
    }
}
