namespace Dometrain.Monolith.Api.Contracts.Requests;

public record StudentRegistrationRequest(string Email, string FullName, string Password);

public record UpdateStudentRequest(string Email, string FullName);
