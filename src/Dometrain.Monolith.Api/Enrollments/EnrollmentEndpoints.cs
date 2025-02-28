using Dometrain.Api.Shared.Identity;
using Dometrain.Monolith.Api.Identity;

namespace Dometrain.Monolith.Api.Enrollments;

public static class EnrollmentEndpoints
{
    public static async Task<IResult> Get(IEnrollmentService enrollmentService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var enrollment = await enrollmentService.GetStudentEnrollmentsAsync(studentId.Value);
        return enrollment is null ? Results.NotFound() : Results.Ok(enrollment);
    }
    
    public static async Task<IResult> Enroll(Guid courseId, IEnrollmentService enrollmentService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var enrolled = await enrollmentService.EnrollToCourseAsync(studentId.Value, courseId);
        return enrolled is null ? Results.NotFound() : Results.Ok();
    }
    
    public static async Task<IResult> UnEnroll(Guid courseId, IEnrollmentService enrollmentService, HttpContext httpContext)
    {
        var studentId = httpContext.GetUserId()!;
        var enrolled = await enrollmentService.UnEnrollFromCourseAsync(studentId.Value, courseId);
        return enrolled is null ? Results.NotFound() : Results.Ok();
    }
}
