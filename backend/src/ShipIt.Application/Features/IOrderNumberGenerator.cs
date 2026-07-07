using ShipIt.Domain.Orders.ValueObjects;

namespace ShipIt.Application.Features;

public interface IOrderNumberGenerator
{
    OrderNumber Generate();
}
