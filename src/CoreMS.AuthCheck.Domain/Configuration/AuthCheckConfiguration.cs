namespace CoreMS.AuthCheck.Domain.Configuration;

public class AuthCheckConfiguration
{
    public const string Position = "AuthCheck";

    /// <summary>
    ///  Delay in minutes to check auth.
    /// </summary>
    public int? DelayInMinutes { get; set; }

    /// <summary>
    ///  OAuth Checks Enabled
    /// </summary>
    public bool OAuth { get; set; } = true;

    /// <summary>
    ///  Checks to perform for auth check.
    /// </summary>
    public List<CheckConfiguration> Checks { get; set; } = new();
}
