namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api result response base class
    /// </summary>
    public class WebApiResultBase : WebApiResponse
    {
        internal const int StatusInternalError = -2147483648;

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
