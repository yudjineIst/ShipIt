using ShipIt.Domain.Common;
using ShipIt.Domain.Orders.ValueObjects;

namespace ShipIt.Domain.Orders;

public sealed class DeliveryOrder
{
    private DeliveryOrder() { }

    private DeliveryOrder(
        Guid id,
        OrderNumber orderNumber,
        City senderCity,
        Address senderAddress,
        City recipientCity,
        Address recipientAddress,
        CargoWeight cargoWeight,
        DateOnly pickupDate,
        DateTime createdAtUtc)
    {
        Id = id;
        OrderNumber = orderNumber;
        SenderCity = senderCity;
        SenderAddress = senderAddress;
        RecipientCity = recipientCity;
        RecipientAddress = recipientAddress;
        CargoWeight = cargoWeight;
        PickupDate = pickupDate;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }
    public OrderNumber OrderNumber { get; private set; } = null!;
    public City SenderCity { get; private set; } = null!;
    public Address SenderAddress { get; private set; } = null!;
    public City RecipientCity { get; private set; } = null!;
    public Address RecipientAddress { get; private set; } = null!;
    public CargoWeight CargoWeight { get; private set; } = null!;
    public DateOnly PickupDate { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static Result<DeliveryOrder> Create(
        Guid id,
        OrderNumber orderNumber,
        string senderCity,
        string senderStreet,
        string senderHouse,
        int? senderApartment,
        string recipientCity,
        string recipientStreet,
        string recipientHouse,
        int? recipientApartment,
        decimal cargoWeight,
        DateOnly pickupDate,
        DateTime createdAtUtc)
    {
        if (pickupDate == default || pickupDate < DateOnly.FromDateTime(createdAtUtc))
            return OrderErrors.ValidationFailed;

        var senderCityResult = City.Create(senderCity);
        if (senderCityResult.IsFailure)
            return senderCityResult.Error;

        var senderAddressResult = Address.Create(senderStreet, senderHouse, senderApartment);
        if (senderAddressResult.IsFailure)
            return senderAddressResult.Error;

        var recipientCityResult = City.Create(recipientCity);
        if (recipientCityResult.IsFailure)
            return recipientCityResult.Error;

        var recipientAddressResult = Address.Create(recipientStreet, recipientHouse, recipientApartment);
        if (recipientAddressResult.IsFailure)
            return recipientAddressResult.Error;

        var cargoWeightResult = CargoWeight.Create(cargoWeight);
        if (cargoWeightResult.IsFailure)
            return cargoWeightResult.Error;

        return new DeliveryOrder(
            id,
            orderNumber,
            senderCityResult.Value,
            senderAddressResult.Value,
            recipientCityResult.Value,
            recipientAddressResult.Value,
            cargoWeightResult.Value,
            pickupDate,
            createdAtUtc);
    }
}
