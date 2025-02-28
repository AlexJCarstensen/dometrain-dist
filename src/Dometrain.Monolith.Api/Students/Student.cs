namespace Dometrain.Monolith.Api.Students;

public class Student
{
    public required Guid Id { get; init; }

    public required string Email { get; init; }
    
    public required string FullName { get; init; }
}
