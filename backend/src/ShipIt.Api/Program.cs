using ShipIt.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddApiLogging();
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.ApplyMigrationsAndSeedAsync();
}

app.UseCors(ServiceCollectionExtensions.FrontendCorsPolicy);
app.UseApiLogging();
app.MapControllers();

await app.RunAsync();

public partial class Program;
