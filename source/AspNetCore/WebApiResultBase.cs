namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api result response base class
    /// </summary>
    public class WebApiResultBase : WebApiResponse
    {
        /// <summary>
        /// Status code
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Detail message
        /// </summary>
        public string? Detail { get; set; }
    }
}
