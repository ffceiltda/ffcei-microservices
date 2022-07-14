namespace FFCEI.Microservices.Models
{
    public interface IEnabledAwareModel : IModel
    {
        bool IsEnabled { get; set; }
    }
}
