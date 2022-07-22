namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Pagination information for search requests
    /// </summary>
    public class WebApiSearchRequestPagination
    {
        /// <summary>
        /// Current page
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Item count (per page)
        /// </summary>
        public int? ItemCount { get; set; }
    }
}
