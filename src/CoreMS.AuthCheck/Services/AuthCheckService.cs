using CoreMS.AuthCheck.Domain.Configuration;
using CoreMS.AuthCheck.Domain.Interfaces;
using CoreMS.AuthCheck.ServiceDefaults;
using Duende.IdentityModel.Client;
using System.Diagnostics;

namespace CoreMS.AuthCheck.Services;

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

        // authchekc
        _logger.LogInformation("AuthCheck: (Name: {name}), (Server: {server}), for (Client ID: {clientId})", oauthCheck.Name, oauthCheck.Server, oauthCheck.ClientId);

        // tag list(s)
        var tagListDiscoDoc = new TagList();
        tagListDiscoDoc.Add(Spans.ClientId, oauthCheck.ClientId);
        tagListDiscoDoc.Add(Spans.Server, oauthCheck.Server);

        var tagListToken = new TagList();        
        tagListToken.Add(Spans.ClientId, oauthCheck.ClientId);        
        tagListToken.Add(Spans.Server, oauthCheck.Server);

        // discovery document
        var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest {
            Address = oauthCheck.Server,
            Policy = {
                RequireHttps = true
            }
        }, cancellationToken);

        if (disco.IsError)
        {
            // failed
            _logger.LogError("Discovery failed: {Error}", disco.Error);

            tagListDiscoDoc.Add(Spans.Result, Spans.Values.Failure);
            OTel.Meters.Auth.AddDiscoveryDocument(1, tagListDiscoDoc);

            activity?.SetStatus(ActivityStatusCode.Error, $"Discovery failed: {disco.Error}");
            throw new InvalidOperationException($"Discovery failed: {disco.Error}");
        } else
        {
            // succeeded
            tagListDiscoDoc.Add(Spans.Result, Spans.Values.Success);
            OTel.Meters.Auth.AddDiscoveryDocument(1, tagListDiscoDoc);
        }

        // token request
        var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest {
                Address = disco.TokenEndpoint,
                ClientId = oauthCheck.ClientId,
                ClientSecret = oauthCheck.ClientSecret,
                Scope = oauthCheck.Scopes
            }, cancellationToken);

        if (tokenResponse.IsError)
        {
            // failure
            _logger.LogError("Token request failed: {Error}", tokenResponse.Error);

            tagListDiscoDoc.Add(Spans.Result, Spans.Values.Failure);
            OTel.Meters.Auth.AddToken(1, tagListDiscoDoc);

            activity?.SetStatus(ActivityStatusCode.Error, $"Token request failed: {tokenResponse.Error}");
            throw new InvalidOperationException($"Token request failed: {tokenResponse.Error}");
        } else
        {
            // success
            tagListDiscoDoc.Add(Spans.Result, Spans.Values.Success);
            OTel.Meters.Auth.AddToken(1, tagListDiscoDoc);

            _logger.LogInformation("AuthCheck: (Name: {name}), (Server: {server}), SUCCESS!", oauthCheck.Name, oauthCheck.Server);
        }

        // passed
        activity?.SetStatus(ActivityStatusCode.Ok);

        return tokenResponse.AccessToken!;
    }
}
