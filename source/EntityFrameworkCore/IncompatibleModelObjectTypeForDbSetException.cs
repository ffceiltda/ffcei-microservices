namespace FFCEI.Microservices.EntityFrameworkCore;

/// <summary>
/// Incompatible object type for DbSet exception
/// </summary>
public sealed class IncompatibleModelObjectTypeForDbSetException : Exception
{
    /// <summary>
    /// Expected Model object type
    /// </summary>
    public Type? ExpectedModelObjectType { get; set; }

    /// <summary>
    /// Used Model object type
    /// </summary>
    public Type? ModelObjectType { get; set; }

    /// <summary>
    /// Default constructur
    /// </summary>
    public IncompatibleModelObjectTypeForDbSetException()
    {
    }

    /// <summary>
    /// Default constructur (with message)
    /// </summary>
    public IncompatibleModelObjectTypeForDbSetException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Default constructur (with message and inner exception)
    /// </summary>
    public IncompatibleModelObjectTypeForDbSetException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Main constructor
    /// </summary>
    /// <param name="expectedModelObjectType">Expected Model object type</param>
    /// <param name="modelObjectType">Model object type</param>
    public IncompatibleModelObjectTypeForDbSetException(Type expectedModelObjectType, Type modelObjectType)
        : base($"Trying to use a incompatible object type {modelObjectType?.ToString()} into a DbSet of type {expectedModelObjectType?.ToString()}")
    {
        ExpectedModelObjectType = expectedModelObjectType;
        ModelObjectType = modelObjectType;
    }
}
