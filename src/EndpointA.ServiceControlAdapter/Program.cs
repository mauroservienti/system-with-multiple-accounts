using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;
using ServiceControl.TransportAdapter;

namespace EndpointA.ServiceControlAdapter
{
    class Program
    {
        static async Task Main()
        {
            var transportAdapterConfig =
                new TransportAdapterConfig<LearningTransport, LearningTransport>("MyTransport");

            string folder = Path.GetTempPath();
            transportAdapterConfig.CustomizeEndpointTransport(
                customization: transportExtensions =>
                {
                    string pathForEndpoint = Path.Combine(folder, $"Application-EndpointA");
                    transportExtensions.StorageDirectory(pathForEndpoint);
                });

            ITransportAdapter transportAdapter = TransportAdapter.Create(transportAdapterConfig);
            await transportAdapter.Start();

            Console.WriteLine("Started ServiceControlAdapter for Endpoint A");
            Console.ReadLine();
            await transportAdapter.Stop();
        }
    }
}