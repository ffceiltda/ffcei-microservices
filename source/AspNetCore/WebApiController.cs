using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api controller base class
    /// </summary>
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class WebApiController : ControllerExtended
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">Default logger</param>
        public WebApiController(ILogger logger)
            : base(logger)
        {
        }
    }
}
