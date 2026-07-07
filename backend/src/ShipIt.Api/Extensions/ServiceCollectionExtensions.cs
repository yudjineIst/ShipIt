using FluentValidation;
using ShipIt.Application.Features;
using ShipIt.Infrastructure;

namespace ShipIt.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public const string FrontendCorsPolicy = "Frontend";
    private const string FrontendOrigin = "http://localhost:5026";

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddControllers();
        services.AddSwaggerGen();
        services.AddProblemDetails();
        services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy =>
            policy.WithOrigins(FrontendOrigin).AllowAnyHeader().AllowAnyMethod()));

        services.AddInfrastructure(configuration);
        services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderValidator>();
        services.AddSingleton<IOrderNumberGenerator, OrderNumberGenerator>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<GetOrdersHandler>();
        services.AddScoped<GetOrderByIdHandler>();

        return services;
    }
}
