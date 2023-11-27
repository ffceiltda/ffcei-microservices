using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Extended controller base class
/// </summary>
public class Controller : ControllerBase
{
    /// <summary>
    /// Default logger
    /// </summary>
    public ILogger Logger { get; private set; }

    /// <summary>
    /// Remote client address
    /// </summary>
    public string? RemoteClientAddress => GetRemoteClientAddress();

    static readonly char[] _splitOptions = [','];

    private string? GetRemoteClientAddress()
    {
        var requestorAddress = Request?.Headers["X-Forwarded-For"].ToString().Split(_splitOptions).FirstOrDefault();

        if (string.IsNullOrEmpty(requestorAddress))
        {
            requestorAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        return requestorAddress;
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="logger">Default logger</param>
    public Controller(ILogger logger)
    {
        Logger = logger;
    }
}
