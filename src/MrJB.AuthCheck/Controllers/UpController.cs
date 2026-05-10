using Microsoft.AspNetCore.Mvc;
using MrJB.AuthCheck.ServiceDefaults;

namespace MrJB.AuthCheck.Controllers;

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
