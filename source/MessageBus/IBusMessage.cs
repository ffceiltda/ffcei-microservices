namespace FFCEI.Microservices.MessageBus
{
    public interface IBusMessage
    {
        void SerializeTo(Stream stream);

        void SerializeTo(Memory<byte> memory);

        void SerializeTo(Span<byte> span);

        void DesserializeFrom(Stream stream);

        void DesserializeFrom(ReadOnlyMemory<byte> memory);

        void DesserializeFrom(ReadOnlySpan<byte> span);
    }
}
