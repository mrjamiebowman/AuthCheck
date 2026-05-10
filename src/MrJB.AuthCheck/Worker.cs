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
        // delay
        var delay = _authCheck.DelayInMinutes ?? 20;

        _logger.LogInformation("AuthCheck Worker started with a delay of {delay}.", delay);

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
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Worker.");
            }

            // delay
            _logger.LogInformation("Delay {delayInMinutes} mins.", delay);
            await Task.Delay(TimeSpan.FromMinutes(delay), stoppingToken);
        }

        _logger.LogInformation("AuthCheck Worker stopped.");
    }
}
