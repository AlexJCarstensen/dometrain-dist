namespace Dometrain.Monolith.Api.Orders;

public class PlaceOrderRequest
{
    public required IEnumerable<Guid> CourseIds { get; init; }
}
