using CoreMS.AuthCheck.Domain.Configuration;
using CoreMS.AuthCheck.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace CoreMS.AuthCheck;

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
        int delay = _authCheck.DelayInMinutes ?? 20;

        // must be 1 minute or more...
        if (delay <= 1)
        {
            delay = 1;
        }

        _logger.LogInformation("AuthCheck Worker started with a delay of {delayInMinutes} in minutes.", delay);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at {Time}", DateTimeOffset.Now);

            // check for authchecks
            if (!_authCheck.OAuthChecks.Any())
            {
                _logger.LogWarning("There aren't any OAuthChecks.");
                goto delay;
            }

            // oauth checks
            foreach (var item in _authCheck.OAuthChecks)
            {
                try
                {
                    // get token
                    var token = await _authCheckService.GetAccessTokenAsync(item, stoppingToken);
                } catch (OperationCanceledException)
                {
                    // Normal shutdown
                } catch (Exception ex) {
                    // suppress
                }
            }

            // delay
            delay:
            _logger.LogInformation("Delay {delayInMinutes} mins.", delay);

            await Task.Delay(TimeSpan.FromMinutes(delay), stoppingToken);
        }

        _logger.LogInformation("AuthCheck Worker stopped.");
    }
}
