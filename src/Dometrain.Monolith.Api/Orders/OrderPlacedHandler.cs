using Dometrain.Monolith.Api.Enrollments;
using MassTransit;

namespace Dometrain.Monolith.Api.Orders;

public class OrderPlacedHandler : IConsumer<OrderPlaced>
{
    private readonly IEnrollmentService _enrollmentService;

    public OrderPlacedHandler(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    public async Task Consume(ConsumeContext<OrderPlaced> context)
    {
        var courseIds = context.Message.CourseIds;
        var studentId = context.Message.StudentId;

        foreach (var courseId in courseIds)
        {
            await _enrollmentService.EnrollToCourseAsync(studentId, courseId);
        }
    }
}
