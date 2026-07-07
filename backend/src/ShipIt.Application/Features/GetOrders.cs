using Microsoft.Extensions.Logging;
using ShipIt.Application.Common.Abstractions;
using ShipIt.Domain.Common;

namespace ShipIt.Application.Features;

public sealed record GetOrdersQuery;

public sealed record OrderListItemResponse(
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

public sealed class GetOrdersHandler(
    IOrderRepository orderRepository,
    ILogger<GetOrdersHandler> logger)
{
    public async Task<Result<IReadOnlyList<OrderListItemResponse>>> Handle(
        GetOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var ordersResult = await orderRepository.GetAllOrderedByCreatedDateDescendingAsync(
            cancellationToken);

        if (ordersResult.IsFailure)
            return Result<IReadOnlyList<OrderListItemResponse>>.Failure(ordersResult.Error);

        var orders = ordersResult.Value
            .Select(order => new OrderListItemResponse(
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
                order.CreatedAtUtc))
            .ToList();

        logger.LogInformation("Retrieved {OrderCount} orders", orders.Count);

        return Result<IReadOnlyList<OrderListItemResponse>>.Success(orders);
    }
}
