
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var mainDbUsername = builder.AddParameter("postgres-username");
var mainDbPassword = builder.AddParameter("postgres-password");

var mainDb = builder.AddPostgres("main-db", mainDbUsername, mainDbPassword, port: 5432)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    //.WithPgAdmin()
    .AddDatabase("dometrain");

var cartAccount = builder.AddAzureCosmosDB("cosmosdb");

builder.AddProject<Projects.Dometrain_Cart_Processor>("dometrain-cart-processor")
    .WithReference(cartAccount).WaitFor(cartAccount);
    
cartAccount.AddCosmosDatabase("cartdb").AddContainer("carts", "/pk");

var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight(resourceBuilder => resourceBuilder.WithLifetime(ContainerLifetime.Persistent));

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

builder.AddContainer("prometheus", "prom/prometheus")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount("../../prometheus", "/etc/prometheus", isReadOnly: true)
    .WithHttpEndpoint(port: 9090, targetPort: 9090);

var grafana = builder.AddContainer("grafana", "grafana/grafana")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount("../../grafana/config", "/etc/grafana", isReadOnly: true)
    .WithBindMount("../../grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
    .WithHttpEndpoint(targetPort: 3000, name: "http");

var mainApi = builder.AddProject<Projects.Dometrain_Monolith_Api>("dometrain-api")
    .WithReference(mainDb).WaitFor(mainDb)
    .WithReference(redis).WaitFor(redis)
    .WithReference(rabbitmq).WaitFor(rabbitmq)
    .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("http"));

// builder.AddProject<Projects.Dometrain_Cart_Api>("cart-api")
//     .WithReference(cartAccount).WaitFor(cartAccount)
//     .WithReference(mainApi).WaitFor(mainApi)
//     .WithReference(redis).WaitFor(redis);

builder.AddContainer("cart-api", "cart.api")
     .WithReference(cartAccount).WaitFor(cartAccount)
     .WithReference(mainApi).WaitFor(mainApi)
     .WithReference(redis).WaitFor(redis);

var app = builder.Build();
    
app.Run();
