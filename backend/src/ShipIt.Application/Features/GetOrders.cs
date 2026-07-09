using FluentValidation;
using Microsoft.Extensions.Logging;
using ShipIt.Application.Common.Abstractions;
using ShipIt.Application.Common.Models;
using ShipIt.Domain.Common;

namespace ShipIt.Application.Features;

public sealed record GetOrdersQuery(int PageNumber = 1, int PageSize = 20)
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
}

public sealed class GetOrdersRequest
{
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }

    public GetOrdersQuery ToQuery()
    {
        var pageNumber = GetOrdersQuery.DefaultPageNumber;
        if (PageNumber.HasValue)
        {
            pageNumber = PageNumber.Value;
        }

        var pageSize = GetOrdersQuery.DefaultPageSize;
        if (PageSize.HasValue)
        {
            pageSize = PageSize.Value;
        }

        return new GetOrdersQuery(pageNumber, pageSize);
    }
}

public sealed class GetOrdersValidator : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, GetOrdersQuery.MaxPageSize);
    }
}

public sealed record OrderListItemResponse(
    Guid Id,
    string OrderNumber,
    string SenderCity,
    string SenderAddress,
    string RecipientCity,
    string RecipientAddress,
    decimal CargoWeight,
    DateOnly PickupDate,
    DateTime CreatedAtUtc);

public sealed class GetOrdersHandler(
    IOrderRepository orderRepository,
    IValidator<GetOrdersQuery> validator,
    ILogger<GetOrdersHandler> logger)
{
    public async Task<Result<PagedResponse<OrderListItemResponse>>> Handle(
        GetOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            var message = string.Join(" ", validation.Errors.Select(error => error.ErrorMessage));
            logger.LogWarning("Orders list validation failed: {ValidationErrors}", message);
            return new Error("Orders.PaginationInvalid", message, ErrorType.Validation);
        }

        var ordersResult = await orderRepository.GetAllOrderedByCreatedDateDescendingAsync(cancellationToken);

        if (ordersResult.IsFailure)
            return ordersResult.Error;

        var orders = ordersResult.Value
            .Select(order => new OrderListItemResponse(
                order.Id,
                order.OrderNumber.Value,
                order.SenderCity.Value,
                FormatAddress(order.SenderAddress.Street, order.SenderAddress.House, order.SenderAddress.Apartment),
                order.RecipientCity.Value,
                FormatAddress(order.RecipientAddress.Street, order.RecipientAddress.House, order.RecipientAddress.Apartment),
                order.CargoWeight.Value,
                order.PickupDate,
                order.CreatedAtUtc))
            .ToList();

        var response = orders.ToPagedResponse(query.PageNumber, query.PageSize);

        logger.LogInformation(
            "Retrieved {OrderCount} orders from page {PageNumber}",
            response.Items.Count,
            query.PageNumber);

        return response;
    }

    private static string FormatAddress(string street, string house, int? apartment)
    {
        var address = $"{street}, {house}";
        if (apartment.HasValue)
        {
            address = $"{address}, кв. {apartment.Value}";
        }

        return address;
    }
}
