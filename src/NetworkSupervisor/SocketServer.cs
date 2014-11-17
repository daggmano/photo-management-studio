using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Shared;

namespace NetworkSupervisor
{
    public delegate void ServerInfoChangedEventHandler(object sender, ServerInfoEventArgs e);

    public class ServerInfoEventArgs : EventArgs
    {
        public IPAddress Address { get; set; }
        public int Port { get; set; }
    }

    public class SocketServer
    {
        public int SocketPort { get; private set; }
        // Thread signal.
        public ManualResetEvent AllDone = new ManualResetEvent(false);

        public event ServerInfoChangedEventHandler OnServerInfoChanged;

        public void StartListening()
        {
            // Establish the local endpoint for the socket.
            var localEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // Create a TCP/IP socket.
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                SocketPort = ((IPEndPoint) listener.LocalEndPoint).Port;

                while (true)
                {
                    // Set the event to nonsignaled state.
                    AllDone.Reset();

                    // Start an asynchronous socket to listen for connections.
//                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(AcceptCallback, listener);

                    // Wait until a connection is made before continuing.
                    AllDone.WaitOne();
                }

            }
            catch (Exception e)
            {
//                Console.WriteLine(e.ToString());
                Debug.WriteLine(e);
            }

//            Console.WriteLine("\nPress ENTER to continue...");
//            Console.Read();
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            AllDone.Set();

            // Get the socket that handles the client request.
            var listener = (Socket)ar.AsyncState;
            var handler = listener.EndAccept(ar);

            // Create the state object.
            var state = new SocketStateObject();
            state.WorkSocket = handler;
            handler.BeginReceive(state.Buffer, 0, SocketStateObject.BufferSize, 0, ReadCallback, state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            var content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            var state = (SocketStateObject)ar.AsyncState;
            var handler = state.WorkSocket;

            // Read data from the client socket. 
            var bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.AppendValue(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.Value;
                if (content.IndexOf("<EOF>", StringComparison.InvariantCulture) > -1)
                {
                    // All the data has been read from the 
                    // client. Display it on the console.
//                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
//                    Console.WriteLine("Remote IP: {0}", ((IPEndPoint)handler.RemoteEndPoint).Address);

                    // Remove <EOF> string
                    var msg = content.Substring(0, content.Length - 5);
                    var obj = JsonConvert.DeserializeObject<NetworkMessageObject>(msg);
                    switch (obj.MessageType)
                    {
                        case NetworkMessageType.ServerSpecification:
                            var spec = JsonConvert.DeserializeObject<NetworkMessageObject<ServerSpecificationObject>>(msg);
                            if (OnServerInfoChanged != null)
                            {
                                OnServerInfoChanged(this, new ServerInfoEventArgs
                                {
                                    Address = ((IPEndPoint)handler.RemoteEndPoint).Address,
                                    Port = spec.Message.ServerPort
                                });
                            }
//                            Console.WriteLine("Remote Port = {0}", spec.Message.ServerPort);
                            break;
                    }
                    // Echo the data back to the client.
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.Buffer, 0, SocketStateObject.BufferSize, 0, ReadCallback, state);
                }
            }
        }

        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            var byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                var handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                var bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
