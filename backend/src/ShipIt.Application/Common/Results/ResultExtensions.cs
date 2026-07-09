using ShipIt.Domain.Common;

namespace ShipIt.Application.Common.Results;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
    {
        if (result.IsSuccess)
            return map(result.Value);

        return result.Error;
    }
}
