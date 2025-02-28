namespace Dometrain.Monolith.Api.Orders;

public class Order
{
    public required Guid Id { get; init; }

    public required Guid StudentId { get; init; }

    public required List<Guid> CourseIds { get; init; } = [];

    public required DateTime CreatedAtUtc { get; init; }
}
