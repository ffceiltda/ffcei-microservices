namespace FFCEI.Microservices.Models
{
    /// <summary>
    /// Model interface with IsEnabled property
    /// </summary>
    public interface IEnabledAwareModel : IModel
    {
        /// <summary>
        /// IsEnabled property
        /// </summary>
        bool IsEnabled { get; set; }
    }
}
