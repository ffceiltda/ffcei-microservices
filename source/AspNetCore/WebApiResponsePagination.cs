namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Pagination information for search responses
/// </summary>
public class WebApiSearchResponsePagination
{
    /// <summary>
    /// Current page
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Page count
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// Item count (per page)
    /// </summary>
    public int ItemCount { get; set; }
}
