namespace FFCEI.Microservices.Models
{
    /// <summary>
    /// Model class with TimeStamping (created / updated) support
    /// </summary>
    public class TimeStampedModel : Model, ITimeStampedModel
    {
        public DateTimeOffset? CreatedAt { get; protected internal set; }

        public DateTimeOffset? UpdatedAt { get; protected internal set; }

        public override void CopyModelPropertiesFrom(IModel model)
        {
            base.CopyModelPropertiesFrom(model);
        }
    }
}
