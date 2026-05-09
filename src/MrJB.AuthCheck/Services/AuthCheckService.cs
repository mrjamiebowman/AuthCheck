using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using MrJB.AuthCheck.Domain.Configuration;
using MrJB.AuthCheck.Domain.Interfaces;

namespace MrJB.AuthCheck.Services;

public class AuthCheckService : IAuthCheckService
{
    // logger
    private readonly ILogger<AuthCheckService> _logger;

    // http client
    private readonly HttpClient _httpClient;

    // configuration
    private readonly OAuthCheckConfiguration _options;

    public AuthCheckService(
        ILogger<AuthCheckService> logger,
        HttpClient httpClient,
        IOptions<OAuthCheckConfiguration> options)
    {
        _logger = logger;
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Requesting OAuth token from {TokenEndpoint}", _options.TokenEndpoint);

        var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = "https://identity.yourdomain.com",
            Policy =
            {
                RequireHttps = true
            }
        }, cancellationToken);

        if (disco.IsError)
        {
            _logger.LogError("Discovery failed: {Error}", disco.Error);
            throw new InvalidOperationException($"Discovery failed: {disco.Error}");
        }

        var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "authcheck",
                ClientSecret = "your-secret",
                Scope = "api.read"
            },
            cancellationToken);

        if (tokenResponse.IsError)
        {
            _logger.LogError("Token request failed: {Error}", tokenResponse.Error);
            throw new InvalidOperationException($"Token request failed: {tokenResponse.Error}");
        }

        return tokenResponse.AccessToken!;
    }
}
