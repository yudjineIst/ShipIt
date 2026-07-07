namespace ShipIt.Client.Models;

public sealed record CreateOrderRequest(
    string SenderCity,
    string SenderStreet,
    string SenderHouse,
    int? SenderApartment,
    string RecipientCity,
    string RecipientStreet,
    string RecipientHouse,
    int? RecipientApartment,
    decimal CargoWeight,
    DateOnly PickupDate);
