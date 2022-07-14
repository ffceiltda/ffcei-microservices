namespace FFCEI.Microservices.Models
{
    public interface ITimeStampedModel : IModel
    {
        DateTimeOffset? CreatedAt { get; }

        DateTimeOffset? UpdatedAt { get; }
    }
}
