using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// WebApiResponse extension methods
    /// </summary>
    public static class WebApiResponseExtensionMethods
    {
        /// <summary>
        /// Returns a IActionResult from a WebApiResponse
        /// </summary>
        /// <typeparam name="TWebApiResponse">WebApiResponse derived class</typeparam>
        /// <param name="response">Response</param>
        /// <returns>NotFound if response is null, OK if response is not null</returns>
        public static IActionResult ToHttpResponse<TWebApiResponse>(this TWebApiResponse response)
            where TWebApiResponse : WebApiResponse
        {
            if (response is null)
            {
                return new NotFoundObjectResult(response);
            }

            return new OkObjectResult(response);
        }

        /// <summary>
        /// Returns a IActionResult from a WebApiResponse
        /// </summary>
        /// <param name="response">Response</param>
        /// <returns>NotFound if response is null or Status is null, OK if response status is 0, InternalError if status if 500, BadRequest if status &gt; 0, NotAcceptable if status &lt; 0</returns>
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

        /// <summary>
        /// Returns a IActionResult from a WebApiResponseWith&lt;TResult&gt;
        /// </summary>
        /// <typeparam name="TResult">Response type</typeparam>
        /// <param name="response">Response</param>
        /// <returns>NotFound if response is null or Status is null, OK if response status is 0, InternalError if status if 500, BadRequest if status &gt; 0, NotAcceptable if status &lt; 0</returns>
        public static IActionResult ToHttpResponse<TResult>(this WebApiResultWith<TResult> response)
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
