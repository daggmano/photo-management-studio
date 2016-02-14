using System.Net.Sockets;
using System.Text;

namespace NetworkSupervisor
{
    public class SocketStateObject
    {
        // Size of receive buffer.
        public const int BufferSize = 1024;
        private readonly StringBuilder _sb;

        // Client  socket.
        public Socket WorkSocket { get; set; }
        // Receive buffer.
        public byte[] Buffer { get; set; }
        // Received data string.
        public string Value
        {
            get { return _sb.ToString(); }
        }

        public SocketStateObject()
        {
            WorkSocket = null;
            Buffer = new byte[BufferSize];
            _sb = new StringBuilder();
        }

        public void AppendValue(string s)
        {
            _sb.Append(s);
        }
    }
}
