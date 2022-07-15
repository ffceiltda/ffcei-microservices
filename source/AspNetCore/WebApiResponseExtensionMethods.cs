using Microsoft.AspNetCore.Mvc;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// WebApiResponse extension methods
    /// </summary>
    public static class WebApiResponseExtensionMethods
    {
        /// <summary>
        /// Creates ActionResult for returning Web Api HTTP responses
        /// </summary>
        /// <typeparam name="TWebApiResponse"></typeparam>
        /// <param name="response">WebApiResponse descentant instance</param>
        /// <returns></returns>
        public static ActionResult<TWebApiResponse> ToHttpResponse<TWebApiResponse>(this TWebApiResponse response)
            where TWebApiResponse : WebApiResponse
        {
            if (response == null)
            {
                return new NotFoundObjectResult(response);
            }

            return new OkObjectResult(response);
        }
    }
}
