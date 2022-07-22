namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Base Web Api Search Request class
    /// </summary>
    public class WebApiSearchRequest : WebApiRequest
    {
        /// <summary>
        /// Dynamic search filter
        /// </summary>
        public WebApiSearchRequestFilter Filter { get; set; } = new();

        /// <summary>
        /// Pagination information
        /// </summary>
        public WebApiSearchRequestPagination Pagination { get; set; } = new();
    }
}
