using System.Globalization;
using ShipIt.Application.Common.Abstractions;
using ShipIt.Domain.Orders.ValueObjects;

namespace ShipIt.Application.Features;

public sealed class OrderNumberGenerator : IOrderNumberGenerator
{
    public const string Prefix = "ORD";
    public const int RandomPartLength = 6;
    public const string DatePartFormat = "yyyyMMdd";

    private readonly IDateTimeProvider dateTimeProvider;

    public OrderNumberGenerator(IDateTimeProvider dateTimeProvider)
    {
        this.dateTimeProvider = dateTimeProvider;
    }

    public OrderNumber Generate()
    {
        DateTime currentDateTime = dateTimeProvider.UtcNow;

        string datePart = CreateDatePart(currentDateTime);
        string randomPart = CreateRandomPart();

        string orderNumberValue = $"{Prefix}-{datePart}-{randomPart}";

        return OrderNumber.Create(orderNumberValue).Value;
    }

    private static string CreateDatePart(DateTime dateTime)
    {
        return dateTime.ToString(DatePartFormat, CultureInfo.InvariantCulture);
    }

    private static string CreateRandomPart()
    {
        string guidValue = Guid.NewGuid().ToString("N");
        string shortGuidValue = guidValue.Substring(0, RandomPartLength);

        return shortGuidValue.ToUpperInvariant();
    }
}