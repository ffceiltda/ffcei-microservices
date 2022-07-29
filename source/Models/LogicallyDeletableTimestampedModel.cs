namespace FFCEI.Microservices.Models;

/// <summary>
/// Model class with IsLogicallyDeleted property and Timestamping (created / updated) support
/// </summary>
public class LogicallyDeletableTimeStampedModel : TimestampedModel, ILogicallyDeletableModel
{
    public bool IsLogicallyDeleted { get; set; }

    public override void CopyModelPropertiesFrom(IModel model)
    {
        base.CopyModelPropertiesFrom(model);

        if (model is ILogicallyDeletableModel modelCasted)
        {
            IsLogicallyDeleted = modelCasted.IsLogicallyDeleted;
        }
    }

    public void LogicallyDelete()
    {
        IsLogicallyDeleted = true;
    }

    public void LogicallyUndelete()
    {
        IsLogicallyDeleted = false;
    }
}
