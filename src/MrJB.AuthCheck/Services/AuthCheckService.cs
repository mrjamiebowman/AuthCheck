using Microsoft.Extensions.Options;
using MrJB.AuthCheck.Domain.Configuration;
using MrJB.AuthCheck.Domain.Interfaces;
using MrJB.AuthCheck.Domain.Models;

namespace MrJB.AuthCheck.Services;

public class AuthCheckService : IAuthCheckService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthCheckService> _logger;
    private readonly OAuthCheckConfiguration _options;

    public AuthCheckService(
        HttpClient httpClient,
        ILogger<AuthCheckService> logger,
        IOptions<OAuthCheckConfiguration> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Requesting OAuth token from {TokenEndpoint}", _options.TokenEndpoint);

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["scope"] = _options.Scope
            })
        };

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogError(
                "OAuth token request failed. StatusCode: {StatusCode}. Body: {Body}",
                response.StatusCode,
                errorBody);

            throw new InvalidOperationException($"OAuth token request failed with status code {response.StatusCode}.");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);

        if (tokenResponse is null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
        {
            _logger.LogError("OAuth token response did not contain an access token.");
            throw new InvalidOperationException("OAuth token response did not contain an access token.");
        }

        _logger.LogInformation("OAuth token received successfully. ExpiresIn: {ExpiresIn} seconds", tokenResponse.ExpiresIn);

        return tokenResponse.AccessToken;
    }
}
