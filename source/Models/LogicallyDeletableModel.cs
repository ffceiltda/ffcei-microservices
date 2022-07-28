namespace FFCEI.Microservices.Models;

/// <summary>
/// Model class with IsLogicallyDeleted property
/// </summary>
public class LogicallyDeletableModel : Model, ILogicallyDeletableModel
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
}
