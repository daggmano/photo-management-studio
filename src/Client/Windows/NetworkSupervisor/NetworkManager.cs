using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared;
using ErrorReporting;

namespace NetworkSupervisor
{
    public class NetworkManager
    {
        private ConnectionState _connectionStatus;
        private Timer _watchdogTimer;
        private SocketServer _socketServer;
        private Thread _socketServerThread;

        private IPAddress _imageServerAddress;
        private int _imageServerPort;

        public event ServerInfoChangedEventHandler OnServerInfoChanged;

        public void Initialize()
        {
            var watchdogTimeout = Int32.Parse(ConfigurationManager.AppSettings["WatchdogTimeout"]);
            
            _connectionStatus = ConnectionState.Disconnected;
            _imageServerAddress = null;
            _imageServerPort = 0;

            _watchdogTimer = new Timer(OnWatchdogTimer, null, 0, watchdogTimeout);

            _socketServer = new SocketServer();
            _socketServer.OnServerInfoChanged += _socketServer_OnServerInfoChanged;
            _socketServerThread = new Thread(_socketServer.StartListening);
            _socketServerThread.Start();
        }

        private void _socketServer_OnServerInfoChanged(object sender, ServerInfoEventArgs e)
        {
            _imageServerAddress = e.Address;
            _imageServerPort = e.Port;

            if (OnServerInfoChanged != null)
            {
                OnServerInfoChanged(this, new ServerInfoEventArgs
                {
                    Address = _imageServerAddress,
                    Port = _imageServerPort
                });
            }

            _connectionStatus = ConnectionState.Connected;
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

        private async void PingServer()
        {
            var client = new HttpClient();
            var url = $"http://{_imageServerAddress}:{_imageServerPort}/api/ping";
            Debug.WriteLine($"Requesting {url}");

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Connection", new[] {"Keep-Alive"});

            try
            {
                var response = await client.SendAsync(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine($"Client disconnected, status code: {response.StatusCode}");
                    _connectionStatus = ConnectionState.Disconnected;
                    _imageServerAddress = null;
                    _imageServerPort = 0;

                    if (OnServerInfoChanged != null)
                    {
                        OnServerInfoChanged(this, new ServerInfoEventArgs
                        {
                            Address = null,
                            Port = 0
                        });
                    }
                }
                else
                {
					var json = await response.Content.ReadAsStringAsync();
					var pingObject = JsonConvert.DeserializeObject<PingResponseObject>(json);
					Debug.WriteLine($"Client received OK from ping at {pingObject.Data.ServerDateTime}");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Client disconnected, exception: {ex}");
				ErrorReporter.SendException(ex);
                _connectionStatus = ConnectionState.Disconnected;
                _imageServerAddress = null;
                _imageServerPort = 0;
            }
        }

        public async Task<ServerInfoResponseObject> GetDbServerId()
        {
            var client = new HttpClient();
            var url = String.Format("http://{0}:{1}/api/serverInfo", _imageServerAddress, _imageServerPort);
            Debug.WriteLine("Requesting " + url);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Connection", new[] { "Keep-Alive" });

            try
            {
                var response = await client.SendAsync(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine($"Client disconnected, status code: {response.StatusCode}");
                }
                else
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var serverIdObject = JsonConvert.DeserializeObject<ServerInfoResponseObject>(json);
                    return serverIdObject;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Client disconnected, exception: {ex}");
				ErrorReporter.SendException(ex);
                _connectionStatus = ConnectionState.Disconnected;
                _imageServerAddress = null;
                _imageServerPort = 0;
            }

            return null;
        }
    }
}
