namespace FFCEI.Microservices.MessageBus
{
    /// <summary>
    /// Bus destination
    /// </summary>
    public class BusDestination
    {
        /// <summary>
        /// Destination type?
        /// </summary>
        public BusDestinationType? DestinationType { get; set; }

        /// <summary>
        /// Destination address
        /// </summary>
        public string? Address { get; set; }
    }
}
