namespace FFCEI.Microservices.Models
{
    public abstract class EnabledAwareTimeStampedModel : TimeStampedModel, IEnabledAwareModel
    {
        public bool IsEnabled { get; set; } = true;

        public override void CopyModelPropertiesFrom(IModel model)
        {
            base.CopyModelPropertiesFrom(model);

            if (model is IEnabledAwareModel modelCasted)
            {
                IsEnabled = modelCasted.IsEnabled;
            }
        }
    }
}
