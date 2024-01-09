using System.Text;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api result response with result data
/// </summary>
public class WebApiResultWith<TResult> : WebApiResultWithBase
{
    /// <summary>
    /// Result data
    /// </summary>
    public TResult? Result { get; set; }

    public override string ResultAsString()
    {
        var result = Result?.ToString();

        if (result is null)
        {
            return string.Empty;
        }

        return result;
    }

    public override byte[] ResultAsByteArray()
    {
        if ((Result is not null) && (Result is byte[] byteArray))
        {
            return byteArray;
        }

        return Encoding.UTF8.GetBytes(ResultAsString());
    }

#pragma warning disable CA1000 // Do not declare static members on generic types
    /// <summary>
    /// Creates a 'succeeded' Web Api result response
    /// </summary>
    /// <param name="result">Result date</param>
    /// <param name="detail">Detail message</param>
    /// <returns>WebApiResultWith&lt;TResult&gt; instance</returns>
    public static WebApiResultWith<TResult> Succeeded(TResult? result = default, string? detail = null)
    {
        return new WebApiResultWith<TResult>()
        {
            Status = StatusSucceeded,
            Detail = detail ?? DetailSuceeded,
            Result = result
        };
    }

    /// <summary>
    /// Creates a 'internal error' Web Api result response
    /// </summary>
    /// <param name="detail">Detail message</param>
    /// <returns>WebApiResultWith&lt;TResult&gt; instance</returns>
    public static WebApiResultWith<TResult> InternalError(string? detail = null)
    {
        return new WebApiResultWith<TResult>()
        {
            Status = StatusInternalError,
            Detail = detail ?? DetailInternalError
        };
    }

    /// <summary>
    /// Creates a 'not found' Web Api result response
    /// </summary>
    /// <param name="detail">Detail message</param>
    /// <returns>WebApiResult instance</returns>
    public static WebApiResultWith<TResult> NotFound(string? detail = null)
    {
        return new WebApiResultWith<TResult>()
        {
            Status = StatusNotFound,
            Detail = detail ?? DetailInternalError
        };
    }

    /// <summary>
    /// Creates a 'error' Web Api result response
    /// </summary>
    /// <param name="status">Status code</param>
    /// <param name="detail">Detail message</param>
    /// <returns>WebApiResultWith&lt;TResult&gt; instance</returns>
    public static WebApiResultWith<TResult> Error(string? detail = null, int? status = null)
    {
        return new WebApiResultWith<TResult>()
        {
            Status = (status is null || status == 0 ? StatusInternalError : status),
            Detail = detail ?? DetailInternalError
        };
    }
#pragma warning restore CA1000 // Do not declare static members on generic types

#pragma warning disable CA2225 // Operator overloads have named alternates
    public static implicit operator WebApiResult(WebApiResultWith<TResult> source) => new() { Status = source?.Status, Detail = source?.Detail };
    public static implicit operator WebApiResultWith<TResult>(WebApiResult source) => new() { Status = source?.Status, Detail = source?.Detail };
#pragma warning restore CA2225 // Operator overloads have named alternates
}
