using NServiceBus;
using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using SharedMessages;

namespace EndpointB
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
            config.EnableInstallers();
            transportConfig.ClientFactory(() => new AmazonSQSClient("secret",
                "secretkey", RegionEndpoint.EUWest2));

            // string folder = Path.GetTempPath();
            // string pathForEndpoint = Path.Combine(folder, $"Application-{endpointName}");
            // transportConfig.StorageDirectory(pathForEndpoint);
            var routingConfig = transportConfig.Routing();
            var bridge = routingConfig.ConnectToRouter("RouterEndpoint");
            bridge.RouteToEndpoint(typeof(AMessage), "EndpointA");

            var endpointInstance = await Endpoint.Start(config);
            await endpointInstance.Publish<SomethingHappened>(happened => happened.WhatHappened = "Boem");

            Console.WriteLine($"Endpoint {endpointName} started.");
            Console.ReadLine();

            await endpointInstance.Stop();
        }
    }
}
