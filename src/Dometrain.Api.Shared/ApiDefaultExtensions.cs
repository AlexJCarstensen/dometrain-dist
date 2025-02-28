using System.Text;
using Dometrain.Api.Shared.ErrorHandling;
using Dometrain.Api.Shared.Identity;
using Dometrain.Api.Shared.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Dometrain.Api.Shared;

public static class ApiDefaultExtensions
{
    public static void AddApiDefaults(this IServiceCollection services, IConfiguration config)
    {
        services.AddEndpointsApiExplorer();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());
        
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config["Identity:Key"]!)),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = config["Identity:Issuer"],
                ValidAudience = config["Identity:Audience"],
                ValidateIssuer = true,
                ValidateAudience = true
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("ApiAdmin", p => p.AddRequirements(new AdminAuthRequirement(config["Identity:AdminApiKey"]!)))
            .AddPolicy("Admin", p => p.RequireAssertion(c => 
                c.User.HasClaim(m => m is { Type: "is_admin", Value: "true" })));

        services.AddScoped<ApiKeyAuthFilter>();

        services.AddProblemDetails();
        services.AddExceptionHandler<ProblemExceptionHandler>();
    }

    public static void MapApiDefaults(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler();

        app.UseAuthentication();
        app.UseAuthorization();
    }
}
