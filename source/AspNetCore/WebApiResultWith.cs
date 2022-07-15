using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api result response with result data
    /// </summary>
    public sealed class WebApiResultWith<TResultData> : WebApiResultBase
        where TResultData : struct
    {
        /// <summary>
        /// Result data
        /// </summary>
        public TResultData? ResultData { get; set; }

#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>
        /// Creates a 'succeeded' Web Api result response
        /// </summary>
        /// <param name="resultData">Result date</param>
        /// <param name="detail">Detail message</param>
        /// <returns>WebApiResultWith&lt;TResultData&gt; instance</returns>
        public static WebApiResultWith<TResultData> Succeeded(TResultData? resultData = null, string? detail = null)
        {
            return new WebApiResultWith<TResultData>()
            {
                Detail = detail,
                ResultData = resultData
            };
        }

        /// <summary>
        /// Creates a 'failed' Web Api result response
        /// </summary>
        /// <param name="resultData">Result date</param>
        /// <param name="detail">Detail message</param>
        /// <returns>WebApiResultWith&lt;TResultData&gt; instance</returns>
        public static WebApiResultWith<TResultData> Failed(TResultData? resultData = null, string? detail = null)
        {
            return new WebApiResultWith<TResultData>()
            {
                Status = -1,
                Detail = detail,
                ResultData = resultData
            };
        }

        /// <summary>
        /// Creates a 'error' Web Api result response
        /// </summary>
        /// <param name="status">Status code</param>
        /// <param name="resultData">Result date</param>
        /// <param name="detail">Detail message</param>
        /// <returns>WebApiResultWith&lt;TResultData&gt; instance</returns>
        public static WebApiResultWith<TResultData> Error(int status, TResultData? resultData = null, string? detail = null)
        {
            return new WebApiResultWith<TResultData>()
            {
                Status = (status == 0 ? -1 : status),
                Detail = detail,
                ResultData = resultData
            };
        }
#pragma warning restore CA1000 // Do not declare static members on generic types

        /// <summary>
        /// Generate HTTP response for Web Api controller with TResultData only
        /// </summary>
        /// <returns>ActionResult&lt;TResultData&gt; instance</returns>
        public ActionResult<TResultData> ToHttpResponseWithResultDataOnly() => Status switch
        {
            0 => new OkObjectResult(ResultData),
            > 0 => new BadRequestObjectResult(ResultData) { Value = $"[{Status}] {Detail}" },
            -1 => new ObjectResult(ResultData) { StatusCode = StatusCodes.Status406NotAcceptable, Value = $"[{Status}] {Detail}" },
            _ => new ObjectResult(ResultData) { StatusCode = StatusCodes.Status500InternalServerError, Value = $"[{Status}] {Detail}" }
        };

        /// <summary>
        /// Generate HTTP response for Web Api controller
        /// </summary>
        /// <returns>ActionResult&lt;WebApiResultWith&lt;TResultData&gt;&gt; instance</returns>
        public ActionResult<WebApiResultWith<TResultData>> ToHttpResponse() => Status switch
        {
            0 => new OkObjectResult(this),
            > 0 => new BadRequestObjectResult(this),
            -1 => new ObjectResult(this) { StatusCode = StatusCodes.Status406NotAcceptable },
            _ => new ObjectResult(this) { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }
}
