namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api result response base class
    /// </summary>
    public class WebApiResultBase : WebApiResponse
    {
        internal const int StatusInternalError = -2147483648;
        internal const string DetailSuceeded = "Succeeded";
        internal const string DetailInternalError = "A unexpected error has ocurred";

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
