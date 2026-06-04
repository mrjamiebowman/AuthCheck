using CoreMS.AuthCheck.ServiceDefaults;
using Microsoft.AspNetCore.Mvc;

namespace CoreMS.AuthCheck.Controllers;

[ApiController]
[Route("[controller]")]
public class UpController : ControllerBase
{
    // logger
    private ILogger<UpController> _logger;

    public UpController(ILogger<UpController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        using var activity = OTel.ActivitySource.StartActivity($"{nameof(UpController)}.{nameof(Index)}");

        _logger.LogInformation("{ClassName}.{MethodName}, UP",
            nameof(UpController),
            nameof(Index)
        );

        return Ok("UP");
    }
}
