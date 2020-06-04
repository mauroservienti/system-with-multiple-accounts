using NServiceBus;
using SharedMessages;
using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
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
            config.SendFailedMessagesTo("error.EndpointA");
            config.AuditProcessedMessagesTo("audit.EndpointA");
            config.SendHeartbeatTo("Particular.ServiceControl.EndpointA");

            var transportConfig = config.UseTransport<SqsTransport>();
            config.EnableInstallers();
            transportConfig.ClientFactory(() => new AmazonSQSClient("key", "secret", RegionEndpoint.EUWest2));
            transportConfig.ClientFactory(() => new AmazonSimpleNotificationServiceClient("key", "secret", RegionEndpoint.EUWest2));

            var routingConfig = transportConfig.Routing();

            var bridge = routingConfig.ConnectToRouter("RouterEndpoint");
            bridge.RouteToEndpoint(typeof(AMessage), "EndpointB");
            bridge.RouteToEndpoint(typeof(FailureMessage), "EndpointB");
            bridge.RegisterPublisher(typeof(SomethingHappened), "EndpointB");

            var endpointInstance = await Endpoint.Start(config);

            Console.WriteLine($"Endpoint {endpointName} started.");
            Console.WriteLine("Press 'A' to send a success message");
            Console.WriteLine("Press 'F' to send a failing message");
            Console.WriteLine("Press 'E' to exit");

            while (true)
            {
                var input = Console.ReadLine();
                if (input.ToUpper() == "E")
                {
                    Console.WriteLine("Shutting down the endpoint...");
                    break;
                }

                if (input.ToUpper() == "A")
                {
                    await endpointInstance.Send(new AMessage() {Message = $"Hi, there. I'm {endpointName}."});
                    Console.WriteLine("Sent successful message.");
                }
                else if (input.ToUpper() == "F")
                {
                    await endpointInstance.Send(new FailureMessage() { Message = $"Hi, there. I'm {endpointName}." });
                    Console.WriteLine("Sent failing message.");
                }
            }

            await endpointInstance.Stop();
        }

    }
}
