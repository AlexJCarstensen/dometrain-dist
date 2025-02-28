using Dapper;
using Dometrain.Monolith.Api.Database;

namespace Dometrain.Monolith.Api.Orders;

public record OrderItemDto(Guid OrderId, Guid CourseId);

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId);

    Task<IEnumerable<Order>> GetAllForStudentAsync(Guid studentId);

    Task<Order?> PlaceAsync(Guid studentId, IEnumerable<Guid> courseIds);
}

public class OrderRepository : IOrderRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public OrderRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var order = await connection.QuerySingleOrDefaultAsync<Order>(
            "select id, student_id StudentId, created_at_utc CreatedAtUtc from orders where id = @orderId",
            new { orderId });
        
        if (order is null)
        {
            return null;
        }
        
        var items = await connection.QueryAsync<OrderItemDto>(
            "select order_id OrderId, course_id CourseId from order_items where order_id = @orderId",
            new { orderId });
        
        order.CourseIds.AddRange(items.Select(x => x.CourseId));
        return order;
    }

    public async Task<IEnumerable<Order>> GetAllForStudentAsync(Guid studentId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var orders = (await connection.QueryAsync<Order>(
            "select id, student_id StudentId, created_at_utc CreatedAtUtc from orders where student_id = @studentId",
            new { studentId })).ToArray();
        
        var items = (await connection.QueryAsync<OrderItemDto>(
            "select order_id OrderId, course_id CourseId from order_items where order_id = any (@orderIds)",
            new { orderIds = orders.Select(x => x.Id).ToArray() })).ToArray();

        foreach (var order in orders)
        {
            var courses = items.Where(x => x.OrderId == order.Id).Select(x => x.CourseId).ToArray();
            order.CourseIds.AddRange(courses);
        }
        return orders;
    }

    public async Task<Order?> PlaceAsync(Guid studentId, IEnumerable<Guid> courseIds)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var transaction = connection.BeginTransaction();

        var order = new Order
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CreatedAtUtc = DateTime.UtcNow,
            CourseIds = courseIds.ToList()
        };
        
        await connection.ExecuteAsync(
            """
            insert into orders (id, student_id, created_at_utc) 
            values (@orderId, @studentId, @createdAtUtc)
            """, new { orderId = order.Id, studentId, createdAtUtc = order.CreatedAtUtc }
            );


        foreach (var courseId in order.CourseIds)
        {
            await connection.ExecuteAsync(
                """
                insert into order_items (order_id, course_id)
                values (@orderId, @courseId)
                """, new { orderId = order.Id, courseId }
            );
        }
        
        transaction.Commit();
        return order;
    }
}
