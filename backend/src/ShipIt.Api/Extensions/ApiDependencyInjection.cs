using ShipIt.Application;
using ShipIt.Infrastructure;

namespace ShipIt.Api.Extensions;

public static class ApiDependencyInjection
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

        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}
