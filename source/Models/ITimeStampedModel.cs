namespace FFCEI.Microservices.Models
{
    /// <summary>
    /// Model interface with TimeStamping (created / updated) support
    /// </summary>
    public interface ITimeStampedModel : IModel
    {
        /// <summary>
        /// CreatedAt property
        /// </summary>
        DateTimeOffset? CreatedAt { get; }

        /// <summary>
        /// UpdatedAt property
        /// </summary>
        DateTimeOffset? UpdatedAt { get; }
    }
}
