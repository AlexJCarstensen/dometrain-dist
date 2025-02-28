using Dometrain.Monolith.Api.Enrollments;
using Dometrain.Monolith.Api.Students;
using MassTransit;

namespace Dometrain.Monolith.Api.Orders;

public interface IOrderService
{
    Task<Order?> GetByIdAsync(Guid orderId);

    Task<IEnumerable<Order>> GetAllForStudentAsync(Guid studentId);

    Task<Order?> PlaceAsync(Guid studentId, IEnumerable<Guid> courseIds);
}

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IBus _bus;
    
    public OrderService(IOrderRepository orderRepository, IStudentRepository studentRepository, IBus bus)
    {
        _orderRepository = orderRepository;
        _studentRepository = studentRepository;
        _bus = bus;
    }

    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _orderRepository.GetByIdAsync(orderId);
    }

    public async Task<IEnumerable<Order>> GetAllForStudentAsync(Guid studentId)
    {
        return await _orderRepository.GetAllForStudentAsync(studentId);
    }

    public async Task<Order?> PlaceAsync(Guid studentId, IEnumerable<Guid> courseIds)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student is null)
        {
            return null;
        }
        
        var order = await _orderRepository.PlaceAsync(studentId, courseIds);

        if (order is null)
        {
            return null;
        }

        await _bus.Publish(new OrderPlaced(order.Id, order.StudentId, order.CourseIds));

        return order;
    }
}

public record OrderPlaced(Guid OrderId, Guid StudentId, IEnumerable<Guid> CourseIds);
