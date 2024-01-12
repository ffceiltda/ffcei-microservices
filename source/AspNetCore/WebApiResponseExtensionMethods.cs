using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// IWebApiResponse extension methods
/// </summary>
public static class IWebApiResponseExtensionMethods
{
    /// <summary>
    /// Returns a IActionResult from a WebApiResponse
    /// </summary>
    /// <typeparam name="TWebApiResponse">WebApiResponse derived class</typeparam>
    /// <param name="response">Response</param>
    /// <returns>NotFound if response is null, OK if response is not null</returns>
    public static IActionResult ToHttpResponse<TWebApiResponse>(this TWebApiResponse response)
        where TWebApiResponse : IWebApiResponse
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
        var httpResponse = ((response is null) || (response.Status == WebApiResultBase.StatusNotFound) || (response.Status == 404))
           ?
           new NotFoundObjectResult(response)
           {
               Value = response is null ? WebApiResultBase.DetailNotFound : response
           }
           :
           response.Status switch
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
        ArgumentNullException.ThrowIfNull(response, nameof(response));

        return ToHttpResponseInternal(response, false);
    }

    /// <summary>
    /// Returns a IActionResult from a WebApiResponseWith&lt;TResult&gt; using TResult value if succeeded
    /// </summary>
    /// <typeparam name="TResult">Response type</typeparam>
    /// <param name="response">Response</param>
    /// <returns>NotFound if response is null or Status is null, OK if response status is 0, InternalError if status if 500, BadRequest if status &gt; 0, NotAcceptable if status &lt; 0</returns>
    public static IActionResult ToHttpResponseAsResult<TResult>(this WebApiResultWith<TResult> response)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));

        return ToHttpResponseInternal(response, true);
    }

    /// <summary>
    /// Returns a IActionResult (FileContentResult) from a WebApiResponseDownload using TResult value if succeeded
    /// </summary>
    /// <param name="response">Download (Response)</param>
    /// <returns>NotFound if response is null or Status is null, OK if response status is 0, InternalError if status if 500, BadRequest if status &gt; 0, NotAcceptable if status &lt; 0</returns>
    public static IActionResult ToHttpResponse(this WebApiResultDownload response)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));

        return ToHttpResponseInternal(response, true);
    }

    private static MediaTypeHeaderValue GetResponseMediaTypeOf<TResult>(WebApiResultWith<TResult> result)
    {
        if (result is WebApiResultDownload download)
        {
            return download.MediaType;
        }

        return (typeof(TResult) == typeof(byte[]) ?
            new MediaTypeHeaderValue("application/octet-stream") :
            new MediaTypeHeaderValue("text/plain") { Charset = "utf-8" });
    }

    private static string? GetResponseFilenameOf<TResult>(WebApiResultWith<TResult> result)
    {
        if (result is WebApiResultDownload download)
        {
            return download.Filename;
        }

        return null;
    }

    private static DateTimeOffset? GetResponseModificationOf<TResult>(WebApiResultWith<TResult> result)
    {
        if (result is WebApiResultDownload download)
        {
            return download.ModifiedAt;
        }

        return null;
    }

    private static IActionResult ToHttpResponseInternal<TResult>(WebApiResultWith<TResult> response, bool forceUsingResult)
    {
        var isPlainType = typeof(TResult).IsValueType || typeof(TResult).IsEnum || typeof(TResult) == typeof(string) || typeof(TResult) == typeof(byte[]);
        var writeResponseAsFileStream = forceUsingResult && isPlainType && (response.Result is not null);
        var httpResponse = ((response is null) || (response.Status == WebApiResultBase.StatusNotFound) || (response.Status == 404))
            ?
            new NotFoundObjectResult(response)
            {
                Value = response is null ? global::FFCEI.Microservices.AspNetCore.WebApiResultBase.DetailNotFound : response
            }
            :
            response.Status switch
            {
                WebApiResultBase.StatusSucceeded => writeResponseAsFileStream ? (IActionResult)
                new FileContentResult(response.ResultAsByteArray(), GetResponseMediaTypeOf(response))
                {
                    FileDownloadName = GetResponseFilenameOf(response),
                    LastModified = GetResponseModificationOf(response)
                }
                :
                new OkObjectResult(response)
                {
                    Value = isPlainType ? response : response.Result
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
