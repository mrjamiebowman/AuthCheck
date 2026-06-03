using MrJB.AuthCheck.Domain.Configuration;

namespace MrJB.AuthCheck.Domain.Interfaces;

public interface IAuthCheckService
{
    Task<string> GetAccessTokenAsync(CheckConfiguration oauthCheck, CancellationToken cancellationToken = default);
}
