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
    public class WebApiController : ControllerBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// Try to obtain requestor address
        /// </summary>
        public string? HttpRequestorAddress

        {
            get
            {
                return GetRequestorAddress();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">Default logger</param>
        public WebApiController(ILogger logger)
        {
            Logger = logger;
        }

        private string? GetRequestorAddress()
        {
            var requestorAddress = Request?.Headers["X-Forwarded-For"].ToString().Split(new char[] { ',' }).FirstOrDefault();

            if (string.IsNullOrEmpty(requestorAddress))
            {
                requestorAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            }

            return requestorAddress;
        }
    }
}
