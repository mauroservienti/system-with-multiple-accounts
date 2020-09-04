using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Unicast.Messages;
using ServiceControl.TransportAdapter;

namespace EndpointB.ServiceControlAdapter
{
    class Program
    {
        static async Task Main()
        {
            // TODO: Change LearningTransport to SqsTransport
            var transportAdapterConfig =
                new TransportAdapterConfig<SqsTransport, LearningTransport>("MyTransport");

            string folder = Path.GetTempPath();
            transportAdapterConfig.EndpointSideAuditQueue = "audit.EndpointB";
            transportAdapterConfig.EndpointSideErrorQueue = "error.EndpointB";
            transportAdapterConfig.EndpointSideControlQueue = "Particular.ServiceControl.EndpointB";

            transportAdapterConfig.CustomizeEndpointTransport(
                transportConfig =>
                {
                    transportConfig.ClientFactory(() => new AmazonSQSClient("key", "secret", RegionEndpoint.EUWest2));
                    transportConfig.ClientFactory(() => new AmazonSimpleNotificationServiceClient("key", "secret", RegionEndpoint.EUWest2));
                    transportConfig.GetSettings().SetupMessageMetadataRegistry();
                });

            transportAdapterConfig.CustomizeServiceControlTransport(
                customization: transportConfig =>
                {
                    // TODO: Point to your ServiceControl account
                    // transportConfig.ClientFactory(() => new AmazonSQSClient("key", "secret", RegionEndpoint.EUWest2));
                    // transportConfig.ClientFactory(() => new AmazonSimpleNotificationServiceClient("key", "secret", RegionEndpoint.EUWest2));
                    transportConfig.GetSettings().SetupMessageMetadataRegistry();
                });

            ITransportAdapter transportAdapter = TransportAdapter.Create(transportAdapterConfig);
            await transportAdapter.Start();

            Console.WriteLine("Started ServiceControlAdapter for Endpoint B");
            Console.ReadLine();
            await transportAdapter.Stop();
        }
    }

    public static class SettingsHolderExtensions
    {
        public static void SetupMessageMetadataRegistry(this NServiceBus.Settings.SettingsHolder settings)
        {
            bool IsMessageType(Type t) => true;
            var messageMetadataRegistry = (MessageMetadataRegistry)Activator.CreateInstance(
                type: typeof(MessageMetadataRegistry),
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                args: new object[] { (Func<Type, bool>)IsMessageType },
                culture: CultureInfo.InvariantCulture);
            settings.Set<MessageMetadataRegistry>(messageMetadataRegistry);
        }
    }
}