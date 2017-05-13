using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using Microsoft.ServiceBus;
using Microsoft.Xrm.Sdk;

namespace RelayService
{
    internal class TwoWayRelay
    {
        public static void Main(string[] args)
        {
            var sh = new ServiceHost(typeof(RemoteService));
            var tcpEndpointBehavior = new TransportClientEndpointBehavior();

            var test = ConfigurationManager.AppSettings["SBusEndpoint"];
            sh.AddServiceEndpoint(
                    typeof(RemoteService).GetInterfaces().First(),
                    new WS2007HttpRelayBinding(),
                    ConfigurationManager.AppSettings["SBusEndpoint"]
                )
                .Behaviors.Add(tcpEndpointBehavior);
            sh.Open();
            Console.WriteLine("TwoWay-listener, press ENTER to close");
            Console.ReadLine();

            sh.Close();
        }
    }

    [ServiceBehavior]
    public class RemoteService : ITwoWayServiceEndpointPlugin
    {
        public string Execute(RemoteExecutionContext c)
        {
            Console.WriteLine(
                $"Entity: {c.PrimaryEntityName}, id: {c.PrimaryEntityId}");
            return "secret@password";
        }
    }
}