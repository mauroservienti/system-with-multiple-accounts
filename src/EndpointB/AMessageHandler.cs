using NServiceBus;
using SharedMessages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EndpointB
{
    class AMessageHandler : IHandleMessages<AMessage>
    {
        public Task Handle(AMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received AMessage from {context.ReplyToAddress}, saying {message.Message}");

            return Task.CompletedTask;
        }
    }
}
