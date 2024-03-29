using System.Text.Json.Serialization;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api result response base class
/// </summary>
public class WebApiResultBase : IWebApiResponse
{
    internal static int? StatusNotFound { get { return null; } }
    internal const int StatusSucceeded = 0;
    internal const int StatusInternalError = int.MinValue;
    internal const string DetailNotFound = "Content not found on server";
    internal const string DetailSuceeded = "Succeeded";
    internal const string DetailInternalError = "A unexpected error has ocurred";
    internal const string DetailStatusForbidden = "Access to resource was denied";
    internal const string DetailStatusUnauthorized = "Access to resource was not authorized";

    /// <summary>
    /// Status code
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Detail message
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// Succeeded
    /// </summary>
    [JsonIgnore]
    public bool HasSucceeded => (Status == StatusSucceeded);

    /// <summary>
    /// Failed
    /// </summary>
    [JsonIgnore]
    public bool HasFailed => !HasSucceeded;
}
