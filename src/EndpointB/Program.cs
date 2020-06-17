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
            config.SendFailedMessagesTo("error.EndpointB");
            config.AuditProcessedMessagesTo("audit.EndpointB");
            config.SendHeartbeatTo("Particular.ServiceControl.EndpointB");

            var recoverability = config.Recoverability();
            recoverability.Immediate(
                customizations: immediate =>
                {
                    immediate.NumberOfRetries(0);
                });
            recoverability.Delayed(delayed => delayed.NumberOfRetries(0));

            var transportConfig = config.UseTransport<SqsTransport>();
            config.EnableInstallers();
            transportConfig.ClientFactory(() => new AmazonSQSClient("key", "secret", RegionEndpoint.EUWest2));

            var routingConfig = transportConfig.Routing();
            var bridge = routingConfig.ConnectToRouter("RouterEndpoint");
            bridge.RouteToEndpoint(typeof(AMessage), "EndpointA");

            var endpointInstance = await Endpoint.Start(config);
            await endpointInstance.Publish<SomethingHappened>(happened => happened.WhatHappened = "This happened: EndpointB started");

            Console.WriteLine($"Endpoint {endpointName} started.");
            Console.ReadLine();

            await endpointInstance.Stop();
        }
    }
}
