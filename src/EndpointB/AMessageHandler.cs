using System;
using System.Threading.Tasks;
using NServiceBus;
using SharedMessages;

namespace EndpointB
{
    class AMessageHandler : IHandleMessages<AMessage>
    {
        public Task Handle(AMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received AMessage from {context.ReplyToAddress}, saying {message.Message}");

            return context.Reply(new ReplyMessage {Message = "Hi back to you"});
        }
    }
}