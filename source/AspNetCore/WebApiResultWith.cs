using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api result response with result data
    /// </summary>
    public sealed class WebApiResultWith<TResult> : WebApiResultBase
        where TResult : struct
    {
        /// <summary>
        /// Result data
        /// </summary>
        public TResult? Result { get; set; }

#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>
        /// Creates a 'succeeded' Web Api result response
        /// </summary>
        /// <param name="result">Result date</param>
        /// <param name="detail">Detail message</param>
        /// <returns>WebApiResultWith&lt;TResult&gt; instance</returns>
        public static WebApiResultWith<TResult> Succeeded(TResult? result = null, string? detail = null)
        {
            return new WebApiResultWith<TResult>()
            {
                Detail = detail,
                Result = result
            };
        }

        /// <summary>
        /// Creates a 'failed' Web Api result response
        /// </summary>
        /// <param name="result">Result date</param>
        /// <param name="detail">Detail message</param>
        /// <returns>WebApiResultWith&lt;TResult&gt; instance</returns>
        public static WebApiResultWith<TResult> Failed(TResult? result = null, string? detail = null)
        {
            return new WebApiResultWith<TResult>()
            {
                Status = -1,
                Detail = detail,
                Result = result
            };
        }

        /// <summary>
        /// Creates a 'error' Web Api result response
        /// </summary>
        /// <param name="status">Status code</param>
        /// <param name="result">Result date</param>
        /// <param name="detail">Detail message</param>
        /// <returns>WebApiResultWith&lt;TResult&gt; instance</returns>
        public static WebApiResultWith<TResult> Error(int status, TResult? result = null, string? detail = null)
        {
            return new WebApiResultWith<TResult>()
            {
                Status = (status == 0 ? -1 : status),
                Detail = detail,
                Result = result
            };
        }
#pragma warning restore CA1000 // Do not declare static members on generic types

        /// <summary>
        /// Generate HTTP response for Web Api controller with TResult only
        /// </summary>
        /// <returns>ActionResult&lt;TResult&gt; instance</returns>
        public ActionResult<TResult> ToHttpResponseWithResultOnly() => Status switch
        {
            0 => new OkObjectResult(Result),
            > 0 => new BadRequestObjectResult(Result) { Value = $"[{Status}] {Detail}" },
            -1 => new ObjectResult(Result) { StatusCode = StatusCodes.Status406NotAcceptable, Value = $"[{Status}] {Detail}" },
            _ => new ObjectResult(Result) { StatusCode = StatusCodes.Status500InternalServerError, Value = $"[{Status}] {Detail}" }
        };

        /// <summary>
        /// Generate HTTP response for Web Api controller
        /// </summary>
        /// <returns>ActionResult&lt;WebApiResultWith&lt;TResult&gt;&gt; instance</returns>
        public ActionResult<WebApiResultWith<TResult>> ToHttpResponse() => Status switch
        {
            0 => new OkObjectResult(this),
            > 0 => new BadRequestObjectResult(this),
            -1 => new ObjectResult(this) { StatusCode = StatusCodes.Status406NotAcceptable },
            _ => new ObjectResult(this) { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }
}
