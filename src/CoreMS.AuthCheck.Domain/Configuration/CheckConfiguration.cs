namespace CoreMS.AuthCheck.Domain.Configuration;

public class CheckConfiguration
{
    public const string Position = "Checks";

    public string? Name { get; set; } = string.Empty;

    public string? Server { get; set; } = string.Empty;

    public string? ClientId { get; set; } = string.Empty;

    public string? ClientSecret { get; set; } = string.Empty;

    public string? Scopes { get; set; } = string.Empty;
}
