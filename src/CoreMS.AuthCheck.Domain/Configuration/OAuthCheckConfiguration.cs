namespace CoreMS.AuthCheck.Domain.Configuration;

public class OAuthCheckConfiguration
{
    public const string Position = "OAuthChecks";

    public string? Name { get; set; } = string.Empty;

    public string? Server { get; set; } = string.Empty;

    public string? ClientId { get; set; } = string.Empty;

    public string? ClientSecret { get; set; } = string.Empty;

    public string? Scopes { get; set; } = string.Empty;
}
