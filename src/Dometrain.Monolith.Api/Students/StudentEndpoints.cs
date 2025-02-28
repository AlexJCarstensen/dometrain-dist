using Dometrain.Api.Shared.Identity;
using Dometrain.Monolith.Api.Contracts.Requests;
using Dometrain.Monolith.Api.Identity;

namespace Dometrain.Monolith.Api.Students;

public static class StudentEndpoints
{
    public const string GetStudentEndpointName = "GetStudent";
    
    public static async Task<IResult> Register(StudentRegistrationRequest request, IStudentService studentService)
    {
        var student = request.MapToStudent();
        var createdStudent = await studentService.CreateAsync(student, request.Password);
        return TypedResults.CreatedAtRoute(createdStudent.MapToResponse(), GetStudentEndpointName, new { idOrEmail = createdStudent!.Id });
    }
    
    public static async Task<IResult> Get(string idOrEmail, IStudentService studentService)
    {
        var isId = Guid.TryParse(idOrEmail, out var id);
        var student = isId ? await studentService.GetByIdAsync(id) : await studentService.GetByEmailAsync(idOrEmail);
        return student is null ? Results.NotFound() : Results.Ok(student.MapToResponse());
    }

    public static async Task<IResult> GetMe(HttpContext httpContext, IStudentService studentService)
    {
        var userId = httpContext.GetUserId();
        if (userId is null)
        {
            return Results.NotFound();
        }
        var student = await studentService.GetByIdAsync(userId.Value);
        return student is null ? Results.NotFound() : Results.Ok(student.MapToResponse());
    }
    
    public static async Task<IResult> GetAll(IStudentService studentService, int pageNumber = 1, int pageSize = 25)
    {
        var students = await studentService.GetAllAsync(pageNumber, pageSize);
        return Results.Ok(students.Select(x => x.MapToResponse()));
    }
    
    public static async Task<IResult> Delete(Guid id, IStudentService studentService)
    {
        var deleted = await studentService.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
