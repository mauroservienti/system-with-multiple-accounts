using NServiceBus;
using System;
using System.Threading.Tasks;

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
            config.UseTransport<LearningTransport>().StorageDirectory(@$"c:\temp\Application-{endpointName}");

            var endpointInstance = await Endpoint.Start(config);

            Console.WriteLine($"Endpoint {endpointName} started.");
            Console.ReadLine();
        }
    }
}
