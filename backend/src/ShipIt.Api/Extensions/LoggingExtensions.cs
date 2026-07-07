using Serilog;

namespace ShipIt.Api.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddApiLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console());

        return builder;
    }

    public static WebApplication UseApiLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}
