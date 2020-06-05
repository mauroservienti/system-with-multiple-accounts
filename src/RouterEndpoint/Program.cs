using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Router;

namespace RouterEndpoint
{
    class Program
    {
        static async Task Main()
        {
            var endpointName = typeof(Program).Namespace;
            Console.Title = endpointName;

            string folder = Path.GetTempPath();
            var config = new RouterConfiguration(endpointName);
            var aSide = config.AddInterface<LearningTransport>("ASideOfTheRouter", transportConfig =>
            {
                string pathForEndpoint = Path.Combine(folder, $"Application-EndpointA");
                transportConfig.StorageDirectory(pathForEndpoint);
            });
            var bSide = config.AddInterface<LearningTransport>("BSideOfTheRouter", transportConfig =>
            {
                string pathForEndpoint = Path.Combine(folder, $"Application-EndpointB");
                transportConfig.StorageDirectory(pathForEndpoint);
            });
            var routingProtocol = config.UseStaticRoutingProtocol();
            routingProtocol.AddForwardRoute("ASideOfTheRouter", "BSideOfTheRouter");
            routingProtocol.AddForwardRoute("BSideOfTheRouter", "ASideOfTheRouter");

            config.AutoCreateQueues();

            var router = Router.Create(config);
            await router.Start();

            Console.WriteLine($"Router {endpointName} started.");
            Console.ReadLine();

            await router.Stop();
        }
    }
}