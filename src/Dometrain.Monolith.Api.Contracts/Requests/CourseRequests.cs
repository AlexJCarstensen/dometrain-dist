namespace Dometrain.Monolith.Api.Contracts.Requests;

public record CreateCourseRequest(string Name, string Description, string Author);

public record UpdateCourseRequest(string Name, string Description, string Author);
