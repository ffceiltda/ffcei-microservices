namespace FFCEI.Microservices.Models;

/// <summary>
/// Model class with IsEnabled property and Timestamping (created / updated) support
/// </summary>
public class EnabledAwareTimestampedModel : TimestampedModel, IEnabledAwareModel
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
