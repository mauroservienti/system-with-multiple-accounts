using System;
using System.Threading.Tasks;
using NServiceBus;
using SharedMessages;

namespace EndpointA
{
    public class ReplyMessageHandler : IHandleMessages<ReplyMessage>
    {
        public Task Handle(ReplyMessage message, IMessageHandlerContext context)
        {

            if (context.MessageHeaders.TryGetValue("NServiceBus.OriginatingEndpoint", out var name))
            {
                Console.WriteLine($"Received reply from {name} with message {message.Message}");
                return Task.CompletedTask;
            }

            Console.WriteLine($"Received reply from somewhere with message {message.Message}");
            return Task.CompletedTask;
        }
    }
}