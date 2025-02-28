namespace Dometrain.Api.Shared.Identity;

public class IdentitySettings
{
    public const string SettingsKey = "Identity";

    public required string Key { get; init; }
    
    public required string AdminApiKey { get; init; }
    
    public required TimeSpan Lifetime { get; init; }
    
    public required string Issuer { get; init; }
    
    public required string Audience { get; init; }
}
