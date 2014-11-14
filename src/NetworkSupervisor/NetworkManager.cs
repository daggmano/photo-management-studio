using System.Net;
using System.Threading;

namespace NetworkSupervisor
{
    public class NetworkManager
    {
        private ConnectionState ConnectionStatus { get; set; }
        private Timer WatchdogTimer { get; set; }
        private IPAddress ServerAddress { get; set; }
        private int Port { get; set; }

        public void Initiate()
        {
            ConnectionStatus = ConnectionState.Disconnected;
            WatchdogTimer = new Timer(OnWatchdogTimer, null, 0, 30000);
        }

        private void OnWatchdogTimer(object state)
        {
            switch (ConnectionStatus)
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
            
        }

        private void PingServer()
        {
            
        }
    }
}
