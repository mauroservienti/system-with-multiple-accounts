using System;
using System.Threading.Tasks;
using NServiceBus;
using SharedMessages;

namespace EndpointA
{
    public class SomethingHappenedHandler : IHandleMessages<SomethingHappened>
    {
        public Task Handle(SomethingHappened message, IMessageHandlerContext context)
        {
            if (context.MessageHeaders.TryGetValue("NServiceBus.OriginatingEndpoint", out var name))
            {
                Console.WriteLine($"RReceived event from {name} saying this happened: {message.WhatHappened}");
                return Task.CompletedTask;
            }

            Console.WriteLine($"Received event saying this happened: {message.WhatHappened}");
            return Task.CompletedTask;
        }
    }
}