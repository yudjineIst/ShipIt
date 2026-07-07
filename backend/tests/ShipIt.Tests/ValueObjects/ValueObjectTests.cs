using FluentAssertions;
using ShipIt.Domain.Orders;
using ShipIt.Domain.Orders.ValueObjects;

namespace ShipIt.Tests.ValueObjects;

public sealed class ValueObjectTests
{
    [Fact]
    public void City_ShouldNormalizeExtraSpaces()
    {
        var result = City.Create("  Нижний   Новгород  ");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("Нижний Новгород");
    }

    [Fact]
    public void CargoWeight_ShouldRejectZero()
    {
        var result = CargoWeight.Create(0);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.CargoWeightNotPositive);
    }

    [Fact]
    public void CargoWeight_ShouldRejectNegativeValue()
    {
        var result = CargoWeight.Create(-1);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.CargoWeightNotPositive);
    }

    [Fact]
    public void Address_ShouldCreateWithoutApartment_WhenApartmentIsNotSpecified()
    {
        var result = Address.Create("  Тверская   улица  ", "10А");

        result.IsSuccess.Should().BeTrue();
        result.Value.Street.Should().Be("Тверская улица");
        result.Value.House.Should().Be("10А");
        result.Value.Apartment.Should().BeNull();
    }

    [Fact]
    public void Address_ShouldRejectZeroApartment()
    {
        var result = Address.Create("Тверская улица", "10", 0);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.ApartmentInvalid);
    }

    [Fact]
    public void Address_ShouldRejectNegativeApartment()
    {
        var result = Address.Create("Тверская улица", "10", -1);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.ApartmentInvalid);
    }
}
