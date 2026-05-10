using Microsoft.Extensions.Options;
using MrJB.AuthCheck.Domain.Configuration;
using MrJB.AuthCheck.Domain.Interfaces;

namespace MrJB.AuthCheck;

public sealed class Worker : BackgroundService
{
    // logger
    private readonly ILogger<Worker> _logger;

    // services
    private readonly IAuthCheckService _authCheckService;

    // config
    private AuthCheckConfiguration _authCheck { get; set; }

    public Worker(ILogger<Worker> logger, IAuthCheckService authCheckService, IOptions<AuthCheckConfiguration> authCheck)
    {
        _logger = logger;
        _authCheckService = authCheckService;
        _authCheck = authCheck.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AuthCheck Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Worker running at {Time}", DateTimeOffset.Now);

                // oauth checks
                foreach (var item in _authCheck.OAuthChecks)
                {
                    try
                    {
                        // get token
                        var token = await _authCheckService.GetAccessTokenAsync(item, stoppingToken);
                    } catch (Exception ex)
                    {
                        throw;
                    }
                }

                // delay
                _logger.LogInformation("Delay {delayInMinutes} mins.", _authCheck.DelayInMinutes);
                await Task.Delay(TimeSpan.FromMinutes(_authCheck.DelayInMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Worker.");
            }
        }

        _logger.LogInformation("AuthCheck Worker stopped.");
    }
}
