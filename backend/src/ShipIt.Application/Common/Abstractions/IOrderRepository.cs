using ShipIt.Domain.Common;
using ShipIt.Domain.Orders;

namespace ShipIt.Application.Common.Abstractions;

public interface IOrderRepository
{
    Task<Result<DeliveryOrder>> AddAsync(
        DeliveryOrder order,
        CancellationToken cancellationToken);

    Task<Result<DeliveryOrder>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<DeliveryOrder>>> GetAllOrderedByCreatedDateDescendingAsync(
        CancellationToken cancellationToken);
}
