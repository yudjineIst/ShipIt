using ShipIt.Domain.Common;

namespace ShipIt.Application.Common.Results;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
    {
        if (result.IsSuccess)
            return Result<TOut>.Success(map(result.Value));

        return Result<TOut>.Failure(result.Error);
    }
}