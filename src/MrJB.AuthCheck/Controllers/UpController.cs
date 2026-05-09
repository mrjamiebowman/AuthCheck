using Microsoft.AspNetCore.Mvc;

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

    public Task Up()
    {
        return Task.FromResult("OK");
    }
}
