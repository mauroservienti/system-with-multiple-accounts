using NServiceBus;
using SharedMessages;
using System;
using System.Threading.Tasks;

namespace EndpointA
{
    class Program
    {
        static async Task Main()
        {
            var endpointName = typeof(Program).Namespace;
            Console.Title = endpointName;

            var config = new EndpointConfiguration(endpointName);
            config.SendFailedMessagesTo("error");
            var transportConfig = config.UseTransport<LearningTransport>();
            transportConfig.StorageDirectory(@$"c:\temp\Application-{endpointName}");
            var routingConfig = transportConfig.Routing();
            routingConfig.RouteToEndpoint(typeof(SharedMessages.AMessage).Assembly, "EndpointB");

            var endpointInstance = await Endpoint.Start(config);

            await endpointInstance.Send(new AMessage() { Message = $"Hi, there. I'm {endpointName}." });

            Console.WriteLine($"Endpoint {endpointName} started.");
            Console.ReadLine();
        }
    }
}
