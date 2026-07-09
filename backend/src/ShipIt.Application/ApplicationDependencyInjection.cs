using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ShipIt.Application.Features;

namespace ShipIt.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderValidator>();
        services.AddScoped<IValidator<GetOrdersQuery>, GetOrdersValidator>();
        services.AddSingleton<IOrderNumberGenerator, OrderNumberGenerator>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<GetOrdersHandler>();
        services.AddScoped<GetOrderByIdHandler>();

        return services;
    }
}
