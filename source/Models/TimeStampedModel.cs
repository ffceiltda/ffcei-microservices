namespace FFCEI.Microservices.Models;

/// <summary>
/// Model class with Timestamping (created / updated) support
/// </summary>
public class TimestampedModel : Model, ITimestampedModel
{
    public DateTimeOffset CreatedAt { get; protected internal set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; protected internal set; } = DateTimeOffset.UtcNow;

    public override void CopyModelPropertiesFrom(IModel model)
    {
        base.CopyModelPropertiesFrom(model);
    }
}
