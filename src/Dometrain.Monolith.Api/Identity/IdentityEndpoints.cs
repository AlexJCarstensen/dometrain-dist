using Dometrain.Api.Shared.Identity;
using Dometrain.Monolith.Api.Students;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Dometrain.Monolith.Api.Identity;

public static class IdentityEndpoints
{
    public static async Task<IResult> Login(
        StudentLoginRequest request, 
        IStudentService studentService,
        IIdentityService identityService,
        IOptions<IdentitySettings> identitySettings)
    {
        var isValid = await studentService.CheckCredentialsAsync(
            request.Email, request.Password);

        if (!isValid)
        {
            throw new ValidationException("Invalid login request");
        }

        var user = await studentService.GetByEmailAsync(request.Email);
        
        var jwt = identityService.GenerateToken(user!.Id, request.Email);
        
        return Results.Ok(new
        {
            token_type = "Bearer",
            access_token = jwt,
            expires_in = identitySettings.Value.Lifetime.TotalSeconds
        });
    }
}
