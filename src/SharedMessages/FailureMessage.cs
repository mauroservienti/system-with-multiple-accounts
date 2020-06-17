using NServiceBus;

namespace SharedMessages
{
    public class FailureMessage :IMessage
    {
        public string Message { get; set; }
    }
}