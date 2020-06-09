﻿using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using NServiceBus;
using ServiceControl.TransportAdapter;

namespace EndpointB.ServiceControlAdapter
{
    class Program
    {
        static async Task Main()
        {
            var transportAdapterConfig =
                new TransportAdapterConfig<SqsTransport, SqsTransport>("MyTransport");

            string folder = Path.GetTempPath();
            transportAdapterConfig.CustomizeEndpointTransport(
                transportConfig =>
                {
                    transportConfig.ClientFactory(() => new AmazonSQSClient("accessKey",
                        "secret", RegionEndpoint.EUWest2));
                });

            ITransportAdapter transportAdapter = TransportAdapter.Create(transportAdapterConfig);
            await transportAdapter.Start();

            Console.WriteLine("Started ServiceControlAdapter for Endpoint B");
            Console.ReadLine();
            await transportAdapter.Stop();
        }
    }
}