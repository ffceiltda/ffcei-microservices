using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FFCEI.Microservices.AspNetCore
{
    public sealed class WebApiResultWith<TResultData> : WebApiResult
        where TResultData : class
    {
        public TResultData? ResultData { get; set; }

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static WebApiResultWith<TResultData> Succeded(TResultData? resultData = null, string? detail = null)
        {
            return new WebApiResultWith<TResultData>()
            {
                Detail = detail,
                ResultData = resultData
            };
        }

        public static WebApiResultWith<TResultData> Failed(TResultData? resultData = null, string? detail = null)
        {
            return new WebApiResultWith<TResultData>()
            {
                Status = -1,
                Detail = detail,
                ResultData= resultData
            };
        }

        public static WebApiResultWith<TResultData> Error(int status, TResultData? resultData = null, string? detail = null)
        {
            return new WebApiResultWith<TResultData>()
            {
                Status = status,
                Detail = detail,
                ResultData = resultData
            };
        }
#pragma warning restore CA1000 // Do not declare static members on generic types

        public new ActionResult<WebApiResultWith<TResultData>> Response => Status switch
        {
            0 => new OkObjectResult(this),
            > 0 => new BadRequestObjectResult(this),
            -1 => new ObjectResult(this) { StatusCode = StatusCodes.Status406NotAcceptable },
            _ => new ObjectResult(this) { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }
}
