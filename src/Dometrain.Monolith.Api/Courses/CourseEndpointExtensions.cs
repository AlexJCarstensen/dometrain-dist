namespace Dometrain.Monolith.Api.Courses;

public static class CourseEndpointExtensions
{
    public static WebApplication MapCourseEndpoints(this WebApplication app)
    {
        app.MapPost("/courses", CourseEndpoints.Create)
            .RequireAuthorization("ApiAdmin", "Admin");

        app.MapGet("/courses/{idOrSlug}", CourseEndpoints.Get)
            .AllowAnonymous();
        
        app.MapGet("/courses", CourseEndpoints.GetAll)
            .AllowAnonymous();
        
        app.MapPut("/courses/{id:guid}", CourseEndpoints.Update)
            .RequireAuthorization("ApiAdmin", "Admin");
        
        app.MapDelete("/courses/{id:guid}", CourseEndpoints.Delete)
            .RequireAuthorization("ApiAdmin", "Admin");
        
        return app;
    }
}
