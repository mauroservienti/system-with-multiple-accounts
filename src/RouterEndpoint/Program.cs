using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Router;
using NServiceBus.Unicast.Messages;

namespace RouterEndpoint
{
    class Program
    {
        static async Task Main()
        {
            var endpointName = typeof(Program).Namespace;
            Console.Title = endpointName;

            //string folder = Path.GetTempPath();
            var config = new RouterConfiguration(endpointName);

            var aSide = config.AddInterface<SqsTransport>("ASideOfTheRouter", transportConfig =>
            {
                transportConfig.ClientFactory(() => new AmazonSQSClient("accessKey",
                    "secret", RegionEndpoint.EUWest2));
                transportConfig.EnableMessageDrivenPubSubCompatibilityMode();
                transportConfig.GetSettings().SetupMessageMetadataRegistry();
            });

            var bSide = config.AddInterface<SqsTransport>("BSideOfTheRouter", transportConfig =>
            {
                transportConfig.ClientFactory(() => new AmazonSQSClient("accessKey",
                    "secret", RegionEndpoint.EUWest2));
                transportConfig.EnableMessageDrivenPubSubCompatibilityMode();
                transportConfig.GetSettings().SetupMessageMetadataRegistry();
            });

            var routingProtocol = config.UseStaticRoutingProtocol();
            routingProtocol.AddForwardRoute("ASideOfTheRouter", "BSideOfTheRouter");
            routingProtocol.AddForwardRoute("BSideOfTheRouter", "ASideOfTheRouter");

            config.AutoCreateQueues();
            //config.Settings.SetupMessageMetadataRegistry();

            var router = Router.Create(config);
            await router.Start();

            Console.WriteLine($"Router {endpointName} started.");
            Console.ReadLine();

            await router.Stop();
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
