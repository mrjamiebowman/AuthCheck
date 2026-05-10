namespace MrJB.AuthCheck.Domain.Configuration;

public class OAuthCheckConfiguration
{
    public const string Position = "OAuthCheck";

    public string? Name { get; set; } = string.Empty;

    public string? Server { get; set; } = string.Empty;

    public string? ClientId { get; set; } = string.Empty;

    public string? ClientSecret { get; set; } = string.Empty;

    public string? Scope { get; set; } = string.Empty;
}
