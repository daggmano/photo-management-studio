using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Shared;

namespace PhotoLibraryImageService
{
    internal class Program
    {
        private static readonly int ListenPort = Int32.Parse(ConfigurationManager.AppSettings["UdpListenPort"]);

        private static void Main(string[] args)
        {
            const string baseAddress = "http://localhost:54321/";

            StartListener();

            // Start OWIN host 
            using (WebApp.Start<Startup>(baseAddress))
            {
                while (true)
                {
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
                    }

//                    Console.WriteLine("Received broadcast from {0} : \n {1}\n", groupEp, Encoding.ASCII.GetString(bytes, 0, bytes.Length));
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
