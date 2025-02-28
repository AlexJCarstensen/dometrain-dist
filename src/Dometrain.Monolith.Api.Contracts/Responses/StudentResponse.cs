namespace Dometrain.Monolith.Api.Contracts.Responses;

public class StudentResponse
{
    public required Guid Id { get; init; }

    public required string Email { get; init; }
    
    public required string FullName { get; init; }
}
