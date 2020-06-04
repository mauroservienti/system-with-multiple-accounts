using NServiceBus;

namespace SharedMessages
{
    public class AMessage : IMessage
    {
        public string Message { get; set; }
    }
}
