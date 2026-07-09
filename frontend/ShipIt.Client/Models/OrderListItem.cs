namespace ShipIt.Client.Models;

public sealed record OrderListItem(
    Guid Id,
    string OrderNumber,
    string SenderCity,
    string SenderAddress,
    string RecipientCity,
    string RecipientAddress,
    decimal CargoWeight,
    DateOnly PickupDate,
    DateTime CreatedAtUtc);
