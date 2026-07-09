using ShipIt.Domain.Common;

namespace ShipIt.Domain.Orders.ValueObjects;

public sealed record OrderNumber
{
    public const int MaxLength = 32;

    public string Value { get; private set; } = string.Empty;

    private OrderNumber()
    {
    }

    private OrderNumber(string value)
    {
        Value = value;
    }

    public static Result<OrderNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return OrderErrors.OrderNumberInvalid;

        string normalized = Normalize(value);

        if (normalized.Length > MaxLength)
            return OrderErrors.OrderNumberInvalid;

        return new OrderNumber(normalized);
    }

    private static string Normalize(string value)
    {
        return value.Trim();
    }

    public override string ToString()
    {
        return Value;
    }
}
