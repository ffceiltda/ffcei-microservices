using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api controller base class
    /// </summary>
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class WebApiController : ControllerBase
    {
    }
}
