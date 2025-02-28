namespace Dometrain.Monolith.Api.Contracts.Responses;

public class CourseResponse
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }
    
    public required string Description { get; init; }

    public required string Slug { get; init; }

    public required string Author { get; init; }
}
