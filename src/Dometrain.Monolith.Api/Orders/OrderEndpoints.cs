using Dometrain.Api.Shared.Identity;
using Dometrain.Monolith.Api.Identity;

namespace Dometrain.Monolith.Api.Orders;

public static class OrderEndpoints
{
    public const string GetOrderEndpointName = "GetOrderById";
    
    public static async Task<IResult> Place(PlaceOrderRequest request, IOrderService orderService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var createdOrder = await orderService.PlaceAsync(studentId.Value, request.CourseIds);

        if (createdOrder is null)
        {
            return Results.NotFound();
        }
        
        return TypedResults.CreatedAtRoute(createdOrder, GetOrderEndpointName, new { orderId = createdOrder.Id });
    }
    
    public static async Task<IResult> Get(Guid orderId, IOrderService orderService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var order = await orderService.GetByIdAsync(orderId);

        if (order is null)
        {
            return Results.NotFound();
        }
        
        if (order.StudentId != studentId && !httpContext.IsAdmin())
        {
            return Results.NotFound();
        }
        return Results.Ok(order);
    }
    
    public static async Task<IResult> GetAllForStudent(IOrderService orderService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var orders = await orderService.GetAllForStudentAsync(studentId.Value);
        return Results.Ok(orders);
    }
}
