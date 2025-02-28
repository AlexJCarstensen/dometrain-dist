using Dometrain.Api.Shared.Identity;

namespace Dometrain.Cart.Api.ShoppingCarts;

public static class ShoppingCartEndpoints
{
    public static async Task<IResult> GetMe(IShoppingCartService shoppingCartService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var cart = await shoppingCartService.GetByIdAsync(studentId.Value);
        return cart is null ? Results.NotFound() : Results.Ok(cart);
    }
    
    public static async Task<IResult> AddCourse(Guid courseId, IShoppingCartService shoppingCartService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var cart = await shoppingCartService.AddCourseAsync(studentId.Value, courseId);
        return cart is null ? Results.NotFound() : Results.Ok(cart);
    }
    
    public static async Task<IResult> RemoveCourse(Guid courseId, IShoppingCartService shoppingCartService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var cart = await shoppingCartService.RemoveItemAsync(studentId.Value, courseId);
        return cart is null ? Results.NotFound() : Results.Ok(cart);
    }
    
    public static async Task<IResult> ClearCart(IShoppingCartService shoppingCartService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var cart = await shoppingCartService.ClearAsync(studentId.Value);
        return cart is null ? Results.NotFound() : Results.Ok(cart);
    }
}
