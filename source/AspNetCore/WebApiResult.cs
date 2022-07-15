using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api result response
    /// </summary>
    public class WebApiResult : WebApiResultBase
    {
        /// <summary>
        /// Creates a 'suceeded' Web Api result response
        /// </summary>
        /// <param name="detail">Detail message</param>
        /// <returns>WebApiResult instance</returns>
        public static WebApiResult Succeeded(string? detail = null)
        {
            return new WebApiResult()
            {
                Detail = detail
            };
        }

        /// <summary>
        /// Creates a 'failed' Web Api result response
        /// </summary>
        /// <param name="detail">Detail message</param>
        /// <returns>WebApiResult instance</returns>
        public static WebApiResult Failed(string? detail = null)
        {
            return new WebApiResult()
            {
                Status = -1,
                Detail = detail
            };
        }

        /// <summary>
        /// Creates a 'error' Web Api result response
        /// </summary>
        /// <param name="status">Status code</param>
        /// <param name="detail">Detail message</param>
        /// <returns>WebApiResult instance</returns>
        public static WebApiResult Error(int status, string? detail = null)
        {
            return new WebApiResult()
            {
                Status = (status == 0 ? -1 : status),
                Detail = detail
            };
        }

        /// <summary>
        /// Generate HTTP response for Web Api controller
        /// </summary>
        /// <returns>ActionResult&lt;WebApiResult&gt; instance</returns>
        public ActionResult<WebApiResult> ToHttpResponse() => Status switch
        {
            0 => new OkObjectResult(this),
            > 0 => new BadRequestObjectResult(this),
            -1 => new ObjectResult(this) { StatusCode = StatusCodes.Status406NotAcceptable },
            _ => new ObjectResult(this) { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }
}

