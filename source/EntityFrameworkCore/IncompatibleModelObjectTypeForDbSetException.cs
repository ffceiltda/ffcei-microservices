namespace FFCEI.Microservices.EntityFrameworkCore
{
    public sealed class IncompatibleModelObjectTypeForDbSetException : Exception
    {
        public Type? ExpectedModelObjectType { get; set; }
        public Type? ModelObjectType { get; set; }

        public IncompatibleModelObjectTypeForDbSetException()
        {
        }

        public IncompatibleModelObjectTypeForDbSetException(string message)
            : base(message)
        {
        }

        public IncompatibleModelObjectTypeForDbSetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public IncompatibleModelObjectTypeForDbSetException(Type expectedModelObjectType, Type modelObjectType)
            : base($"Trying to use a incompatible object type {modelObjectType?.ToString()} into a DbSet of type {expectedModelObjectType?.ToString()}")
        {
            ExpectedModelObjectType = expectedModelObjectType;
            ModelObjectType = modelObjectType;
        }
    }
}
