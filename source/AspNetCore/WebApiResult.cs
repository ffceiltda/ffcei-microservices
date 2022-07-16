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
                Detail = detail ?? DetailSuceeded
            };
        }

        /// <summary>
        /// Creates a 'internal error' Web Api result response
        /// </summary>
        /// <param name="detail">Detail message</param>
        /// <returns>WebApiResult instance</returns>
        public static WebApiResult InternalError(string? detail = null)
        {
            return new WebApiResult()
            {
                Status = StatusInternalError,
                Detail = detail ?? DetailInternalError
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
                Status = (status == 0 ? StatusInternalError : status),
                Detail = detail ?? DetailInternalError
            };
        }
    }
}

