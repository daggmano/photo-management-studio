using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Shared;
using ErrorReporting;

namespace PhotoLibraryImageService
{
    public static class SocketClient
    {
        public static void Send(IPAddress ipAddress, int port, INetworkMessageObject value)
        {
            var message = JsonConvert.SerializeObject(value);
            message += "<EOF>";
            var bytes = new byte[1024];

            var remoteEp = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP  socket.
            var sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                sender.Connect(remoteEp);

//                Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint);

                // Encode the data string into a byte array.
                var msg = Encoding.ASCII.GetBytes(message);

                // Send the data through the socket.
                /*var bytesSent =*/ sender.Send(msg);

                // Receive the response from the remote device.
                var bytesRec = sender.Receive(bytes);
                Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

                // Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane);
				ErrorReporter.SendException(ane);
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se);
				ErrorReporter.SendException(se);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e);
				ErrorReporter.SendException(e);
            }
        }
    }
}
