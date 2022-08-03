namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api result response
/// </summary>
public sealed class WebApiResult : WebApiResultBase
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
            Status = StatusSucceeded,
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
    /// Creates a 'not found' Web Api result response
    /// </summary>
    /// <param name="detail">Detail message</param>
    /// <returns>WebApiResult instance</returns>
    public static WebApiResult NotFound(string? detail = null)
    {
        return new WebApiResult()
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
    /// <returns>WebApiResult instance</returns>
    public static WebApiResult Error(string? detail = null, int? status = null)
    {
        return new WebApiResult()
        {
            Status = (status is null || status == 0 ? StatusInternalError : status),
            Detail = detail ?? DetailInternalError
        };
    }
}
