using ShipIt.Domain.Common;

namespace ShipIt.Domain.Orders.ValueObjects;

public sealed record Address
{
    public const int StreetMinLength = 2;
    public const int StreetMaxLength = 200;
    public const int HouseMaxLength = 20;

    public string Street { get; private set; } = string.Empty;
    public string House { get; private set; } = string.Empty;
    public int? Apartment { get; private set; }

    private Address()
    {
    }

    private Address(string street, string house, int? apartment)
    {
        Street = street;
        House = house;
        Apartment = apartment;
    }

    public static Result<Address> Create(string? street, string? house, int? apartment = null)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result<Address>.Failure(OrderErrors.StreetRequired);
        }

        if (string.IsNullOrWhiteSpace(house))
        {
            return Result<Address>.Failure(OrderErrors.HouseRequired);
        }

        var normalizedStreet = Normalize(street);
        var normalizedHouse = house.Trim();

        if (normalizedStreet.Length < StreetMinLength || normalizedStreet.Length > StreetMaxLength)
        {
            return Result<Address>.Failure(OrderErrors.StreetInvalid);
        }

        if (normalizedHouse.Length > HouseMaxLength)
        {
            return Result<Address>.Failure(OrderErrors.HouseInvalid);
        }

        if (apartment <= 0)
        {
            return Result<Address>.Failure(OrderErrors.ApartmentInvalid);
        }

        return Result<Address>.Success(new Address(normalizedStreet, normalizedHouse, apartment));
    }

    private static string Normalize(string value)
    {
        return string.Join(
            " ",
            value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
