using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipIt.Application.Common.Abstractions;
using ShipIt.Domain.Common;
using ShipIt.Domain.Orders;

namespace ShipIt.Infrastructure.Database;

public sealed class OrderRepository : IOrderRepository
{
    private const int SqliteUniqueConstraintErrorCode = 2067;

    private readonly ShipItDbContext _dbContext;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(
        ShipItDbContext dbContext,
        ILogger<OrderRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<DeliveryOrder>> AddAsync(
        DeliveryOrder order,
        CancellationToken cancellationToken)
    {
        _dbContext.DeliveryOrders.Add(order);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<DeliveryOrder>.Success(order);
        }
        catch (DbUpdateException exception)
        {
            SqliteException? sqliteException = GetSqliteException(exception);

            if (sqliteException is not null && IsUniqueConstraintViolation(sqliteException))
            {
                return HandleUniqueConstraintViolation(sqliteException, order);
            }

            _logger.LogError(exception, "Failed to create order {OrderId}", order.Id);

            return Result<DeliveryOrder>.Failure(OrderErrors.DatabaseFailure);
        }
    }

    public async Task<Result<DeliveryOrder>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _dbContext.DeliveryOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

            if (order is null)
                return Result<DeliveryOrder>.Failure(OrderErrors.NotFound(id));

            return Result<DeliveryOrder>.Success(order);
        }
        catch (SqliteException exception)
        {
            _logger.LogError(exception, "Failed to received order {OrderId}", id);
            return Result<DeliveryOrder>.Failure(OrderErrors.DatabaseFailure);
        }
    }

    public async Task<Result<IReadOnlyList<DeliveryOrder>>> GetAllOrderedByCreatedDateDescendingAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _dbContext.DeliveryOrders
                .AsNoTracking()
                .OrderByDescending(order => order.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<DeliveryOrder>>.Success(orders);
        }
        catch (SqliteException exception)
        {
            _logger.LogError(exception, "Failed to received orders");
            return Result<IReadOnlyList<DeliveryOrder>>.Failure(OrderErrors.DatabaseFailure);
        }
    }

    private static SqliteException? GetSqliteException(DbUpdateException exception)
    {
        return exception.InnerException as SqliteException;
    }

    private static bool IsUniqueConstraintViolation(SqliteException exception)
    {
        return exception.SqliteExtendedErrorCode == SqliteUniqueConstraintErrorCode;
    }

    private Result<DeliveryOrder> HandleUniqueConstraintViolation(
        SqliteException exception,
        DeliveryOrder order)
    {
        _logger.LogWarning(
            exception,
            "Unique constraint violation while creating order {OrderId} with number {OrderNumber}",
            order.Id,
            order.OrderNumber.Value);

        return Result<DeliveryOrder>.Failure(OrderErrors.OrderNumberConflict);
    }
}