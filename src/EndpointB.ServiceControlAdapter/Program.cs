using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;
using ServiceControl.TransportAdapter;

namespace EndpointB.ServiceControlAdapter
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
                    string pathForEndpoint = Path.Combine(folder, $"Application-EndpointB");
                    transportExtensions.StorageDirectory(pathForEndpoint);
                });

            ITransportAdapter transportAdapter = TransportAdapter.Create(transportAdapterConfig);
            await transportAdapter.Start();

            Console.WriteLine("Started ServiceControlAdapter for Endpoint B");
            Console.ReadLine();
            await transportAdapter.Stop();
        }
    }
}