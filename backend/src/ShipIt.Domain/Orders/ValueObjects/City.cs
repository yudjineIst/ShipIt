using ShipIt.Domain.Common;

namespace ShipIt.Domain.Orders.ValueObjects;

public sealed record City
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    public string Value { get; private set; } = string.Empty;

    private City()
    {
    }

    private City(string value)
    {
        Value = value;
    }

    public static Result<City> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<City>.Failure(OrderErrors.CityRequired);

        string normalized = Normalize(value);

        if (normalized.Length < MinLength || normalized.Length > MaxLength)
            return Result<City>.Failure(OrderErrors.CityInvalid);

        return Result<City>.Success(new City(normalized));
    }

    private static string Normalize(string value)
    {
        return string.Join(
            " ",
            value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    public override string ToString()
    {
        return Value;
    }
}
