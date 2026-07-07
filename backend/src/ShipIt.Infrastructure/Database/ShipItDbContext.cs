using Microsoft.EntityFrameworkCore;
using ShipIt.Domain.Orders;

namespace ShipIt.Infrastructure.Database;

public sealed class ShipItDbContext(DbContextOptions<ShipItDbContext> options)
    : DbContext(options)
{
    public DbSet<DeliveryOrder> DeliveryOrders => Set<DeliveryOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShipItDbContext).Assembly);
    }
}
