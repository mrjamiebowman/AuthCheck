using CoreMS.AuthCheck.Domain.Configuration;

namespace CoreMS.AuthCheck.Domain.Interfaces;

public interface IAuthCheckService
{
    Task<string> GetAccessTokenAsync(OAuthCheckConfiguration oauthCheck, CancellationToken cancellationToken = default);
}
