using Microsoft.Extensions.Logging;
using ShipIt.Application.Common.Abstractions;
using ShipIt.Domain.Common;
using ShipIt.Domain.Orders;

namespace ShipIt.Application.Features;

public sealed record GetOrderByIdQuery(Guid Id);

public sealed record OrderDetailsResponse(
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

public sealed class GetOrderByIdHandler(
    IOrderRepository orderRepository,
    ILogger<GetOrderByIdHandler> logger)
{
    public async Task<Result<OrderDetailsResponse>> Handle(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken)
    {
        var orderResult = await orderRepository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (orderResult.IsFailure)
        {
            if (orderResult.Error.Type == ErrorType.NotFound)
                logger.LogWarning("Order {OrderId} was not found", query.Id);

            return orderResult.Error;
        }

        var order = orderResult.Value;
        var response = new OrderDetailsResponse(
            order.Id,
            order.OrderNumber.Value,
            order.SenderCity.Value,
            order.SenderAddress.Street,
            order.SenderAddress.House,
            order.SenderAddress.Apartment,
            order.RecipientCity.Value,
            order.RecipientAddress.Street,
            order.RecipientAddress.House,
            order.RecipientAddress.Apartment,
            order.CargoWeight.Value,
            order.PickupDate,
            order.CreatedAtUtc);

        logger.LogInformation("Order {OrderId} was retrieved", query.Id);
        return response;
    }
}
