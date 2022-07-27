using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api controller base class
    /// </summary>
    [ApiController]
    public class WebApiController : Controller
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
