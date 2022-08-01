namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Base Web Api Search Response class
/// </summary>
public class WebApiSearchResponse : IWebApiResponse
{
    /// <summary>
    /// Pagination information
    /// </summary>
    public WebApiSearchResponsePagination Pagination { get; set; } = new();
}
