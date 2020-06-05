using NServiceBus;
using System;
using System.IO;
using System.Threading.Tasks;
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

            var transportConfig = config.UseTransport<LearningTransport>();
            string folder = Path.GetTempPath();
            string pathForEndpoint = Path.Combine(folder, $"Application-{endpointName}");
            transportConfig.StorageDirectory(pathForEndpoint);

            var routingConfig = transportConfig.Routing();
            routingConfig.RouteToEndpoint(typeof(SharedMessages.AMessage).Assembly, "EndpointA");

            var endpointInstance = await Endpoint.Start(config);
            await endpointInstance.Publish<SomethingHappened>(happened => happened.WhatHappened = "Boem");

            Console.WriteLine($"Endpoint {endpointName} started.");
            Console.ReadLine();

            await endpointInstance.Stop();
        }
    }
}
