namespace FFCEI.Microservices.Models
{
    /// <summary>
    /// Model class with IsEnabled property
    /// </summary>
    public class EnabledAwareModel : Model, IEnabledAwareModel
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
