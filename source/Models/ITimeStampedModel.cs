namespace FFCEI.Microservices.Models
{
    /// <summary>
    /// Model interface with Timestamping (created / updated) support
    /// </summary>
    public interface ITimestampedModel : IModel
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
