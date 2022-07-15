namespace FFCEI.Microservices.AspNetCore
{
    /// <summary>
    /// Web Api claims base class
    /// </summary>
    public class WebApiClaims
    {
        /// <summary>
        /// Authentication Uuid (for log purposes to correlate requests)
        /// </summary>
        public Guid? AuthenticationUuid { get; set; }
    }
}
