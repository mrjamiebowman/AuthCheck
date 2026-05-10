using MrJB.AuthCheck.Domain.Configuration;

namespace MrJB.AuthCheck.Domain.Interfaces;

public interface IAuthCheckService
{
    Task<string> GetAccessTokenAsync(OAuthCheckConfiguration oauthCheck, CancellationToken cancellationToken = default);
}
