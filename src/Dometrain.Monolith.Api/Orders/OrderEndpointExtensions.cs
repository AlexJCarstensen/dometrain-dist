namespace Dometrain.Monolith.Api.Orders;

public static class OrderEndpointExtensions
{
    public static WebApplication MapOrderEndpoints(this WebApplication app)
    {
        app.MapGet("/orders", OrderEndpoints.GetAllForStudent)
            .RequireAuthorization();
        
        app.MapGet("/orders/{orderId:guid}", OrderEndpoints.Get)
            .WithName(OrderEndpoints.GetOrderEndpointName)
            .RequireAuthorization();
        
        app.MapPost("/orders", OrderEndpoints.Place)
            .RequireAuthorization();
        
        return app;
    }
}
