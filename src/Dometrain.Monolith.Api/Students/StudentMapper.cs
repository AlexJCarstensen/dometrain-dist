using Dometrain.Monolith.Api.Contracts.Requests;
using Dometrain.Monolith.Api.Contracts.Responses;

namespace Dometrain.Monolith.Api.Students;

public static class StudentMapper
{
    public static Student MapToStudent(this StudentRegistrationRequest request)
    {
        return new Student
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.FullName
        };
    }
    
    public static StudentResponse? MapToResponse(this Student? student)
    {
        if (student is null)
        {
            return null;
        }
    
        return new StudentResponse
        {
            Id = student.Id,
            Email = student.Email,
            FullName = student.FullName
        };
    }
}
