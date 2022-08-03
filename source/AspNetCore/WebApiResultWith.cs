namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api result response with result data
/// </summary>
public sealed class WebApiResultWith<TResult> : WebApiResultBase
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
    /// <param name="result">Result date</param>
    /// <param name="detail">Detail message</param>
    /// <returns>WebApiResultWith&lt;TResult&gt; instance</returns>
    public static WebApiResultWith<TResult> InternalError(TResult? result = default, string? detail = null)
    {
        return new WebApiResultWith<TResult>()
        {
            Status = StatusInternalError,
            Detail = detail ?? DetailInternalError,
            Result = result
        };
    }

    /// <summary>
    /// Creates a 'not found' Web Api result response
    /// </summary>
    /// <param name="result">Result date</param>
    /// <param name="detail">Detail message</param>
    /// <returns>WebApiResult instance</returns>
    public static WebApiResultWith<TResult> NotFound(TResult? result = default, string? detail = null)
    {
        return new WebApiResultWith<TResult>()
        {
            Status = StatusNotFound,
            Detail = detail ?? DetailInternalError,
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
    public static WebApiResultWith<TResult> Error(TResult? result = default, string? detail = null, int? status = null)
    {
        return new WebApiResultWith<TResult>()
        {
            Status = (status is null || status == 0 ? StatusInternalError : status),
            Detail = detail ?? DetailInternalError,
            Result = result
        };
    }
#pragma warning restore CA1000 // Do not declare static members on generic types

#pragma warning disable CA2225 // Operator overloads have named alternates
    public static implicit operator WebApiResult(WebApiResultWith<TResult> source) => new() { Status = source?.Status, Detail = source?.Detail };
    public static implicit operator WebApiResultWith<TResult>(WebApiResult source) => new() { Status = source?.Status, Detail = source?.Detail };
#pragma warning restore CA2225 // Operator overloads have named alternates
}
