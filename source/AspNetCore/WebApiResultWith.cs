using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api result response with result data
    /// </summary>
    public sealed class WebApiResultWith<TResult> : WebApiResultBase
        where TResult : class
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
        /// Creates a 'internal error' Web Api result response
        /// </summary>
        /// <param name="result">Result date</param>
        /// <param name="detail">Detail message</param>
        /// <returns>WebApiResultWith&lt;TResult&gt; instance</returns>
        public static WebApiResultWith<TResult> InternalError(TResult? result = null, string? detail = null)
        {
            return new WebApiResultWith<TResult>()
            {
                Status = StatusInternalError,
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
                Status = (status == 0 ? StatusInternalError : status),
                Detail = detail,
                Result = result
            };
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }
}
