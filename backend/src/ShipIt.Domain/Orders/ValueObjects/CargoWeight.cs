using System.Globalization;
using ShipIt.Domain.Common;

namespace ShipIt.Domain.Orders.ValueObjects;

public sealed record CargoWeight
{
    public const decimal MinValue = 0m;
    public const int TotalDigits = 10;
    public const int FractionDigits = 3;

    public decimal Value { get; private set; }

    private CargoWeight()
    {
    }

    private CargoWeight(decimal value)
    {
        Value = value;
    }

    public static Result<CargoWeight> Create(decimal value)
    {
        if (value <= MinValue)
            return OrderErrors.CargoWeightNotPositive;

        return new CargoWeight(value);
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
