namespace FFCEI.Microservices.Models
{
    public interface IModel
    {
        void CopyModelPropertiesFrom(IModel model);
    }
}
