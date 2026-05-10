using Duende.IdentityModel.Client;
using MrJB.AuthCheck.Domain.Configuration;
using MrJB.AuthCheck.Domain.Interfaces;

namespace MrJB.AuthCheck.Services;

public class AuthCheckService : IAuthCheckService
{
    // logger
    private readonly ILogger<AuthCheckService> _logger;

    // http client
    private readonly HttpClient _httpClient;

    public AuthCheckService(
        ILogger<AuthCheckService> logger,
        HttpClient httpClient
        )
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<string> GetAccessTokenAsync(OAuthCheckConfiguration oauthCheck, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Requesting OAuth token from {TokenEndpoint}", oauthCheck.TokenEndpoint);

        var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = oauthCheck.TokenEndpoint,
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
                ClientId = oauthCheck.ClientId,
                ClientSecret = oauthCheck.ClientSecret,
                Scope = oauthCheck.Scope
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
