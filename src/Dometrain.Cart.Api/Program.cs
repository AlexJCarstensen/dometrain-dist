using System.Reflection;
using Dometrain.Api.Shared;
using Dometrain.Cart.Api.ShoppingCarts;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

//builder.AddServiceDefaults();

builder.Services.AddMetrics();

builder.Services.AddApiDefaults(config);

builder.Services.AddDometrainApi("http://dometrain-api", config["Identity:AdminApiKey"]!);

builder.AddAzureCosmosClient("cosmosdb");
builder.AddRedisClient("redis");

builder.Services.AddSingleton<IShoppingCartService, ShoppingCartService>();
builder.Services.AddSingleton<ShoppingCartRepository>();
builder.Services.AddSingleton<IShoppingCartRepository>(x =>
    new CachedShoppingCartRepository(x.GetRequiredService<ShoppingCartRepository>(), x.GetRequiredService<IConnectionMultiplexer>()));

AddOpenTelemetry(builder.Services, config);

var app = builder.Build();

//app.MapDefaultEndpoints();
app.MapApiDefaults();

app.MapShoppingCartEndpoints();

app.Run();



static void AddOpenTelemetry(IServiceCollection services, IConfiguration configuration)
{
    const string serviceName = "Process.Api";

    var openTelemetryEndpoint = new Uri(configuration.GetSection("OpenTelemetry:Endpoint").Value!);


    services.AddOpenTelemetry()
        .ConfigureResource(resource =>
        {
            resource
                .AddService(serviceName, serviceVersion: Assembly.GetExecutingAssembly().GetName().Version!.ToString());
        })
        .WithTracing(tracing =>
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                    {
                        options.Endpoint = openTelemetryEndpoint;
                        options.Protocol = OtlpExportProtocol.Grpc;
                    }
                )
        )
        .WithMetrics(metrics =>
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                // Metrics provides by ASP.NET
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                .AddOtlpExporter(options =>
                    {
                        options.Endpoint = openTelemetryEndpoint;
                        options.Protocol = OtlpExportProtocol.Grpc;
                    }
                )
        )
        .WithLogging(
            logging =>
                logging
                    .AddOtlpExporter(options =>
                        {
                            options.Endpoint = openTelemetryEndpoint;
                            options.Protocol = OtlpExportProtocol.Grpc;
                        }
                    )
        );
}
