using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using NServiceBus;
using ServiceControl.TransportAdapter;

namespace EndpointA.ServiceControlAdapter
{
    class Program
    {
        static async Task Main()
        {
            var transportAdapterConfig =
                new TransportAdapterConfig<SqsTransport, SqsTransport>("MyTransport");

            string folder = Path.GetTempPath();
            transportAdapterConfig.CustomizeEndpointTransport(
                customization: transportExtensions =>
                {
                    transportExtensions.ClientFactory(() => new AmazonSQSClient("AKIAU65LPVK4UO7QBD7V", "2QC8+TdzQtb52VP41ufBZ4L38kAlG2X75bMm1d6h", RegionEndpoint.EUWest2));
                });

            ITransportAdapter transportAdapter = TransportAdapter.Create(transportAdapterConfig);
            await transportAdapter.Start();

            Console.WriteLine("Started ServiceControlAdapter for Endpoint A");
            Console.ReadLine();
            await transportAdapter.Stop();
        }
    }
}