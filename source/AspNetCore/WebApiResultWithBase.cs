namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api result response with result data
/// </summary>
public abstract class WebApiResultWithBase : WebApiResultBase
{
    /// <summary>
    /// Return result contents as string
    /// </summary>
    /// <returns>Result contents as string</returns>
    public abstract string ResultAsString();

    /// <summary>
    /// Return result contents as byte array
    /// </summary>
    /// <returns>Result contents as byte array</returns>
    public abstract byte[] ResultAsByteArray();
}
