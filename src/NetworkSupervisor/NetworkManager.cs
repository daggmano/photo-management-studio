using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Shared;

namespace NetworkSupervisor
{
    public class NetworkManager
    {
        private ConnectionState _connectionStatus;
        private Timer _watchdogTimer;
        private SocketServer _socketServer;
        private Thread _socketServerThread;

        public void Initialize()
        {
            var watchdogTimeout = Int32.Parse(ConfigurationManager.AppSettings["WatchdogTimeout"]);
            _connectionStatus = ConnectionState.Disconnected;
            _watchdogTimer = new Timer(OnWatchdogTimer, null, 0, watchdogTimeout);

            _socketServer = new SocketServer();
            _socketServerThread = new Thread(_socketServer.StartListening);
            _socketServerThread.Start();
        }

        private void OnWatchdogTimer(object state)
        {
            switch (_connectionStatus)
            {
                case ConnectionState.Disconnected:
                    AttemptConnection();
                    break;

                case ConnectionState.Connected:
                    PingServer();
                    break;
            }
        }

        private void AttemptConnection()
        {
            if (_socketServer.SocketPort == 0)
            {
                return;
            }

            // Send UDP request out to server
            var udpSearchPort = Int32.Parse(ConfigurationManager.AppSettings["UdpSearchPort"]);

            var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);

            var discoveryObject = new NetworkDiscoveryObject
            {
                Identifier = "Photo.Management.Studio",
                ClientSocketPort = _socketServer.SocketPort
            };

            var sendbuf = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(discoveryObject));
            var ep = new IPEndPoint(IPAddress.Broadcast, udpSearchPort);

            s.SendTo(sendbuf, ep);
        }

        private void PingServer()
        {
            
        }
    }
}
