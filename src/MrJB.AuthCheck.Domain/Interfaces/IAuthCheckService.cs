namespace MrJB.AuthCheck.Domain.Interfaces;

public interface IAuthCheckService
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
