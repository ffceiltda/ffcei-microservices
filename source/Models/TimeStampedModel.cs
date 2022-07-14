namespace FFCEI.Microservices.Models
{
    public abstract class TimeStampedModel : Model, ITimeStampedModel
    {
        public DateTimeOffset? CreatedAt { get; protected internal set; }

        public DateTimeOffset? UpdatedAt { get; protected internal set; }

        public override void CopyModelPropertiesFrom(IModel model)
        {
            base.CopyModelPropertiesFrom(model);
        }
    }
}
