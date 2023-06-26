using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.AspNetCore;

[AllowAnonymous]
[Route("/")]
public class HealthCheckController : WebApiController
{
    public HealthCheckController(ILogger<HealthCheckController> logger)
        : base(logger)
    {
    }

    [HttpOptions]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Options()
    {
        return Ok(new { Running = "yes" });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Get()
    {
        return Ok(new { Running = "yes" });
    }
}
