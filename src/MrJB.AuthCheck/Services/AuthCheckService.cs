using Duende.IdentityModel.Client;
using MrJB.AuthCheck.Domain.Configuration;
using MrJB.AuthCheck.Domain.Interfaces;
using MrJB.AuthCheck.ServiceDefaults;
using System.Diagnostics;

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
        using var activity = OTel.ActivitySource.StartActivity($"{nameof(AuthCheckService)}.{nameof(GetAccessTokenAsync)}");

        _logger.LogInformation("Requesting OAuth token from (Server: {TokenEndpoint}) for (Client ID: {clientId})", oauthCheck.Server, oauthCheck.ClientId);

        // tag list(s)
        var tagListDiscoDoc = new TagList();
        var tagListToken = new TagList();

        tagListDiscoDoc.Add(Spans.ClientId, oauthCheck.ClientId);
        tagListToken.Add(Spans.ClientId, oauthCheck.ClientId);

        tagListDiscoDoc.Add(Spans.Server, oauthCheck.Server);
        tagListToken.Add(Spans.Server, oauthCheck.Server);

        var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest {
            Address = oauthCheck.Server,
            Policy = {
                RequireHttps = true
            }
        }, cancellationToken);

        if (disco.IsError)
        {
            _logger.LogError("Discovery failed: {Error}", disco.Error);

            tagListDiscoDoc.Add(Spans.Result, Spans.Values.Failure);
            OTel.Meters.Auth.AddDiscoveryDocument(1, tagListDiscoDoc);

            throw new InvalidOperationException($"Discovery failed: {disco.Error}");
        } else
        {
            tagListDiscoDoc.Add(Spans.Result, Spans.Values.Success);
            OTel.Meters.Auth.AddDiscoveryDocument(1, tagListDiscoDoc);
        }

        var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest {
                Address = disco.TokenEndpoint,
                ClientId = oauthCheck.ClientId,
                ClientSecret = oauthCheck.ClientSecret,
                Scope = oauthCheck.Scope
            }, cancellationToken);

        if (tokenResponse.IsError)
        {
            _logger.LogError("Token request failed: {Error}", tokenResponse.Error);

            tagListDiscoDoc.Add(Spans.Result, Spans.Values.Failure);
            OTel.Meters.Auth.AddToken(1, tagListDiscoDoc);

            throw new InvalidOperationException($"Token request failed: {tokenResponse.Error}");
        } else
        {
            tagListDiscoDoc.Add(Spans.Result, Spans.Values.Success);
            OTel.Meters.Auth.AddToken(1, tagListDiscoDoc);
        }

        return tokenResponse.AccessToken!;
    }
}
