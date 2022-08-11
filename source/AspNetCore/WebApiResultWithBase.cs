namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api result response with result data
/// </summary>
public abstract class WebApiResultWithBase : WebApiResultBase
{
    public abstract string ResultAsString();
}
