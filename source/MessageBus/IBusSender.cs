namespace FFCEI.Microservices.MessageBus
{
    public interface IBusSender
    {
        void SetDefaultDestination(BusDestination destination);

        void AsyncSend(IBusMessage message);

        Task AsyncSendTo(BusDestination destination, IBusMessage message);
    }
}
