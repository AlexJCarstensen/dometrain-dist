namespace Dometrain.Monolith.Api.Enrollments;

public class Enrollments
{
    public required Guid StudentId { get; init; }

    public List<Guid> CourseIds { get; set; } = [];
}
