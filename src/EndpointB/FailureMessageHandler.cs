using System;
using System.Threading.Tasks;
using NServiceBus;
using SharedMessages;

namespace EndpointB
{
    class FailureMessageHandler : IHandleMessages<FailureMessage>
    {
        public Task Handle(FailureMessage message, IMessageHandlerContext context)
        {
            throw new Exception("this message fails");
        }
    }
}