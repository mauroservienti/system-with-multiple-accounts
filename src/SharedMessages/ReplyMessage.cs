using NServiceBus;

namespace SharedMessages
{
    public class ReplyMessage : IMessage
    {
        public string Message { get; set; }
    }
}