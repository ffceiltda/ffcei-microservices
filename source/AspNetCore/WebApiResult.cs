using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FFCEI.Microservices.AspNetCore
{
    public class WebApiResult : WebApiResponse
    {
        public int Status { get; set; }
        public string? Detail { get; set; }

        public static WebApiResult Succeded(string? detail = null)
        {
            return new WebApiResult()
            {
                Detail = detail
            };
        }

        public static WebApiResult Failed(string? detail = null)
        {
            return new WebApiResult()
            {
                Status = -1,
                Detail = detail
            };
        }

        public static WebApiResult Error(int status, string? detail = null)
        {
            return new WebApiResult()
            {
                Status = status,
                Detail = detail
            };
        }

        public ActionResult<WebApiResult> Response => Status switch {
            0 => new OkObjectResult(this),
            > 0 => new BadRequestObjectResult(this),
            -1 => new ObjectResult(this) { StatusCode = StatusCodes.Status406NotAcceptable },
            _ => new ObjectResult(this) { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }
}
