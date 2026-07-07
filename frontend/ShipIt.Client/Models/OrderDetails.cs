namespace ShipIt.Client.Models;

public sealed record OrderDetails(
    Guid Id,
    string OrderNumber,
    string SenderCity,
    string SenderStreet,
    string SenderHouse,
    int? SenderApartment,
    string RecipientCity,
    string RecipientStreet,
    string RecipientHouse,
    int? RecipientApartment,
    decimal CargoWeight,
    DateOnly PickupDate,
    DateTime CreatedAtUtc);
