namespace Dometrain.Cart.Api.ShoppingCarts;

public static class ShoppingCartEndpointExtensions
{
    public static WebApplication MapShoppingCartEndpoints(this WebApplication app)
    {
        app.MapGet("/cart/me", ShoppingCartEndpoints.GetMe)
            .RequireAuthorization();
        
        app.MapPost("/cart/me/courses/{courseId:guid}", ShoppingCartEndpoints.AddCourse)
            .RequireAuthorization();
        
        app.MapDelete("/cart/me/courses/{courseId:guid}", ShoppingCartEndpoints.RemoveCourse)
            .RequireAuthorization();
        
        app.MapDelete("/cart/me", ShoppingCartEndpoints.ClearCart)
            .RequireAuthorization();
        
        return app;
    }
}
