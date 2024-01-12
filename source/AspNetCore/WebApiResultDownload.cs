using Microsoft.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api result response with download
/// </summary>
public sealed class WebApiResultDownload : WebApiResultWith<byte[]>
{
    /// <summary>
    /// Download media type
    /// </summary>
    [JsonIgnore]
    public MediaTypeHeaderValue MediaType { get; set; } = new MediaTypeHeaderValue("application/octet-stream");

    /// <summary>
    /// Filename for download
    /// </summary>
    [JsonIgnore]
    public string? Filename { get; set; }

    /// <summary>
    /// File modified date/time
    /// </summary>
    [JsonIgnore]
    public DateTimeOffset? ModifiedAt { get; set; }

#pragma warning disable CA1000 // Do not declare static members on generic types
    /// <summary>
    /// Creates a 'succeeded' Web Api result response
    /// </summary>
    /// <param name="download">Download contents</param>
    /// <param name="filename">Filename</param>
    /// <param name="modifiedAt">Modified Date/Time</param>
    /// <param name="mediaType">Media Type</param>
    /// <param name="detail">Detail message</param>
    /// <returns>WebApiResultWith&lt;TResult&gt; instance</returns>
    public static WebApiResultDownload Succeeded(byte[] download, string? filename = null, DateTimeOffset? modifiedAt = null, MediaTypeHeaderValue? mediaType = null, string? detail = null)
    {
        var result = new WebApiResultDownload()
        {
            Status = StatusSucceeded,
            Detail = detail ?? DetailSuceeded,
            Result = download,
            Filename = filename,
            ModifiedAt = modifiedAt
        };

        if (mediaType is not null)
        {
            result.MediaType = mediaType;
        }

        return result;
    }
#pragma warning restore CA1000 // Do not declare static members on generic types

#pragma warning disable CA2225 // Operator overloads have named alternates
    public static implicit operator WebApiResult(WebApiResultDownload source) => new() { Status = source?.Status, Detail = source?.Detail };
    public static implicit operator WebApiResultDownload(WebApiResult source) => new() { Status = source?.Status, Detail = source?.Detail };
#pragma warning restore CA2225 // Operator overloads have named alternates
}
