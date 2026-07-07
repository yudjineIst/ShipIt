using ShipIt.Application.Common.Abstractions;

namespace ShipIt.Tests.Infrastructure;

public sealed class TestDateTimeProvider(DateTime utcNow) : IDateTimeProvider
{
    public DateTime UtcNow { get; set; } = utcNow;
}
