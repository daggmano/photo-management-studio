using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Shared;

namespace PhotoLibraryImageService
{
    internal class Program
    {
        private static readonly int ListenPort = Int32.Parse(ConfigurationManager.AppSettings["UdpListenPort"]);
        private static readonly int PhotoServerPort = Int32.Parse(ConfigurationManager.AppSettings["PhotoServerPort"]);

        private static void Main(string[] args)
        {
            var listenerThread = new Thread(StartListener);
            listenerThread.Start();

            StartWebApi();
        }

        private static async void StartWebApi()
        {
            Func<IPAddress, string> createIpString = (a) => String.Format("http://{0}:{1}", a, PhotoServerPort);

            var ipAddresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
            var options = new StartOptions(createIpString(IPAddress.Loopback));
            foreach (var address in ipAddresses)
            {
                options.Urls.Add(createIpString(address));
            }

            // Start OWIN host 
            using (WebApp.Start<Startup>(options))
            {
                while (true)
                {
                    await Task.Delay(10);
                }
            }
        }

        private static void StartListener()
        {
            var listener = new UdpClient(ListenPort);
            var groupEp = new IPEndPoint(IPAddress.Any, ListenPort);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    var bytes = listener.Receive(ref groupEp);
                    var s = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    var discoveryObject = JsonConvert.DeserializeObject<NetworkDiscoveryObject>(s);
                    if (discoveryObject != null)
                    {
                        Console.WriteLine("IP Address: " + groupEp.Address);
                        Console.WriteLine("Socket Port: " + discoveryObject.ClientSocketPort);

                        var serverSpecification = new ServerSpecificationObject
                        {
                            ServerAddress = "127.0.0.1",
                            ServerPort = PhotoServerPort
                        };
                        var networkMessage = new NetworkMessageObject<ServerSpecificationObject>
                        {
                            MessageType = NetworkMessageType.ServerSpecification,
                            Message = serverSpecification
                        };

                        SocketClient.Send(groupEp.Address, discoveryObject.ClientSocketPort, networkMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
