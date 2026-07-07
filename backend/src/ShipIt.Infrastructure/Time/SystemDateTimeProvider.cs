using ShipIt.Application.Common.Abstractions;

namespace ShipIt.Infrastructure.Time;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
