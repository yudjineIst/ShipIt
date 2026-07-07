using ShipIt.Domain.Common;

namespace ShipIt.Domain.Orders;

public static class OrderErrors
{
    public static readonly Error ValidationFailed = new("Orders.ValidationFailed", "Order validation failed.", ErrorType.Validation);
    public static readonly Error CityRequired = new("Orders.CityRequired", "City is required.", ErrorType.Validation);
    public static readonly Error CityInvalid = new("Orders.CityInvalid", "City length is invalid.", ErrorType.Validation);
    public static readonly Error StreetRequired = new("Orders.StreetRequired", "Street is required.", ErrorType.Validation);
    public static readonly Error StreetInvalid = new("Orders.StreetInvalid", "Street length is invalid.", ErrorType.Validation);
    public static readonly Error HouseRequired = new("Orders.HouseRequired", "House is required.", ErrorType.Validation);
    public static readonly Error HouseInvalid = new("Orders.HouseInvalid", "House length is invalid.", ErrorType.Validation);
    public static readonly Error ApartmentInvalid = new("Orders.ApartmentInvalid", "Apartment must be positive.", ErrorType.Validation);
    public static readonly Error CargoWeightNotPositive = new("Orders.CargoWeightNotPositive", "Cargo weight must be positive.", ErrorType.Validation);
    public static readonly Error OrderNumberInvalid = new("Orders.OrderNumberInvalid", "Order number is invalid.", ErrorType.Validation);
    public static readonly Error OrderNumberConflict = new("Orders.OrderNumberConflict", "An order with this number already exists.", ErrorType.Conflict);
    public static readonly Error DatabaseFailure = new("Orders.DatabaseFailure", "An error occurred while accessing orders.", ErrorType.Failure);

    public static Error NotFound(Guid id) => new("Orders.NotFound", $"Order '{id}' was not found.", ErrorType.NotFound);
}
