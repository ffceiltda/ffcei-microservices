namespace FFCEI.Microservices.Models;

/// <summary>
/// Model interface class
/// </summary>
public interface IModel
{
    /// <summary>
    /// Copy model properties for another model instance
    /// </summary>
    /// <param name="model">IModel instance</param>
    void CopyModelPropertiesFrom(IModel model);
}
