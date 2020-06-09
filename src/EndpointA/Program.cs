using NServiceBus;
using SharedMessages;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;

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
            config.AuditProcessedMessagesTo("audit");
            config.SendHeartbeatTo("Particular.ServiceControl");

            var transportConfig = config.UseTransport<SqsTransport>();

            // string folder = Path.GetTempPath();
            // string pathForEndpoint = Path.Combine(folder, $"Application-{endpointName}");
            // transportConfig.StorageDirectory(pathForEndpoint);
            var routingConfig = transportConfig.Routing();

            var bridge = routingConfig.ConnectToRouter("RouterEndpoint");
            bridge.RouteToEndpoint(typeof(AMessage), "EndpointB");
            bridge.RegisterPublisher(typeof(SomethingHappened), "EndpointB");

            var endpointInstance = await Endpoint.Start(config);
            await endpointInstance.Send(new AMessage() { Message = $"Hi, there. I'm {endpointName}." });

            Console.WriteLine($"Endpoint {endpointName} started.");
            Console.ReadLine();

            await endpointInstance.Stop();
        }

    }
}
