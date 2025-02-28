using Dometrain.Monolith.Api.Contracts.Requests;

namespace Dometrain.Monolith.Api.Courses;

public static class CourseEndpoints
{
    public static async Task<IResult> Create(CreateCourseRequest request, ICourseService courseService)
    {
        var course = request.MapToCourse();
        var createdCourse = await courseService.CreateAsync(course);
        return Results.Ok(createdCourse?.MapToResponse());
    }
    
    public static async Task<IResult> Get(string idOrSlug, ICourseService courseService)
    {
        var isId = Guid.TryParse(idOrSlug, out var id);
        var course = isId ? await courseService.GetByIdAsync(id) : await courseService.GetBySlugAsync(idOrSlug);
        return course is null ? Results.NotFound() : Results.Ok(course.MapToResponse());
    }
    
    public static async Task<IResult> GetAll(ICourseService courseService, 
        string nameFilter = "", int pageNumber = 1, int pageSize = 25)
    {
        var courses = await courseService.GetAllAsync(nameFilter, pageNumber, pageSize);
        return Results.Ok(courses.Select(x => x.MapToResponse()));
    }
    
    public static async Task<IResult> Update(Guid id, UpdateCourseRequest request, ICourseService courseService)
    {
        var course = request.MapToCourse(id);
        var createdCourse = await courseService.UpdateAsync(course);
        return Results.Ok(createdCourse?.MapToResponse());
    }
    
    public static async Task<IResult> Delete(Guid id, ICourseService courseService)
    {
        var deleted = await courseService.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
