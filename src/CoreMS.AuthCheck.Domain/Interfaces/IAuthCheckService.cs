using CoreMS.AuthCheck.Domain.Configuration;

namespace CoreMS.AuthCheck.Domain.Interfaces;

public interface IAuthCheckService
{
    Task<string> GetAccessTokenAsync(CheckConfiguration oauthCheck, CancellationToken cancellationToken = default);
}
