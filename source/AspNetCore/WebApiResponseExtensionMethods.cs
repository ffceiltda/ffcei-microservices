using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// WebApiResponse extension methods
    /// </summary>
    public static class WebApiResponseExtensionMethods
    {
        public static IActionResult ToHttpResponse<TWebApiResponse>(this TWebApiResponse response)
            where TWebApiResponse : WebApiResponse
        {
            if (response is null)
            {
                return new NotFoundObjectResult(response);
            }

            return new OkObjectResult(response);
        }

        public static IActionResult ToHttpResponse(this WebApiResult response)
        {
            if (response is null)
            {
                return new NotFoundObjectResult(response)
                {
                    Value = WebApiResultBase.DetailNotFound
                };
            }

            if (response.Status == WebApiResultBase.StatusNotFound)
            {
                return new NotFoundObjectResult(response)
                {
                    Value = response
                };
            }

            var httpResponse = response.Status switch
            {
                WebApiResultBase.StatusSucceeded => new OkObjectResult(response)
                {
                    Value = response
                },
                WebApiResultBase.StatusInternalError => new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Value = response
                },
                > 0 => new BadRequestObjectResult(response)
                {
                    Value = response
                },
                _ => new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status406NotAcceptable,
                    Value = response
                }
            };

            return httpResponse;
        }

        public static IActionResult ToHttpResponse<TResult>(this WebApiResultWith<TResult> response)
            where TResult : class
        {
            if (response is null)
            {
                return new NotFoundObjectResult(response)
                {
                    Value = WebApiResultBase.DetailNotFound
                };
            }

            if (response.Status == WebApiResultBase.StatusNotFound)
            {
                return new NotFoundObjectResult(response)
                {
                    Value = response
                };
            }

            var httpResponse = response.Status switch
            {
                WebApiResultBase.StatusSucceeded => new OkObjectResult(response)
                {
                    Value = response.Result
                },
                WebApiResultBase.StatusInternalError => new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Value = response
                },
                > 0 => new BadRequestObjectResult(response)
                {
                    Value = response
                },
                _ => new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status406NotAcceptable,
                    Value = response
                }
            };

            return httpResponse;
        }
    }
}
