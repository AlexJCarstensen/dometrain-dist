using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dometrain.Api.Shared.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Dometrain.Monolith.Api.Identity;

public interface IIdentityService
{
    string GenerateToken(Guid userId, string email);
}

public class IdentityService : IIdentityService
{
    private readonly IOptions<IdentitySettings> _identitySettings;

    public IdentityService(IOptions<IdentitySettings> identitySettings)
    {
        _identitySettings = identitySettings;
    }

    public string GenerateToken(Guid userId, string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_identitySettings.Value.Key);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, email),
            new(JwtRegisteredClaimNames.Email, email),
            new("user_id", userId.ToString())
        };

        if (email.EndsWith("@dometrain.com"))
        {
            // User is Dometrain employee
            var claim = new Claim("is_admin", "true", ClaimValueTypes.Boolean);
            claims.Add(claim);
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_identitySettings.Value.Lifetime),
            Issuer = _identitySettings.Value.Issuer,
            Audience = _identitySettings.Value.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
