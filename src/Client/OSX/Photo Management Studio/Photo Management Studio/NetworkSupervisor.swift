//
//  NetworkSupervisor.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 6/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation
import CocoaAsyncSocket

enum ConnectionState {
    case Disconnected
    case Connecting
    case Connected
}

protocol ServerInfoReceivedDelegate {
    func onServerInfoReceived(message: NSData)
}

class NetworkSupervisor : NSObject, ServerInfoReceivedDelegate {
    
    var _connectionStatus: ConnectionState
    var _watchdogTimer: NSTimer?
    
    var _socketIn: InSocket!
    var _socketOut: OutSocket!
    
    var _serverPort: UInt16!
    
    var _imageServerAddress: String?
    var _imageServerPort: Int32
    
    required override init() {
        _connectionStatus = .Disconnected
        _imageServerAddress = nil
        _imageServerPort = 0

        super.init();
        
        _socketIn = InSocket.init(delegate: self, port: 0)
        _serverPort = _socketIn.getPort()
        NSLog("Server Port is \(_serverPort)")
        _socketOut = OutSocket.init(ipAddress: "255.255.255.255", port: Config.udpSearchPort)
    }
    
    func initialize() {
        let watchdogTimeout = Config.watchdogTimeout
  
        _watchdogTimer = NSTimer.scheduledTimerWithTimeInterval(Double(watchdogTimeout) / 1000.0, target: self, selector: "onWatchdogTimer:", userInfo: nil, repeats: true)
    }
    
    func onWatchdogTimer(sender: NSTimer!) {
        switch (_connectionStatus)
        {
        case .Disconnected:
            print(".Disconnected")
            attemptConnection()
            break;
            
        case .Connected:
            print(".Connected")
            pingServer()
            break
            
        default:
            break
        }
    }
    
    func onServerInfoReceived(message: NSData) {
        _connectionStatus = .Connecting
        
        do {
            if let json = try NSJSONSerialization.JSONObjectWithData(message, options: .AllowFragments) as? [String: AnyObject] {
                let networkMessage = NetworkMessageObject.init(json: json)
            
                if let messageType = networkMessage.messageType() {
                    switch (messageType) {
                        case .ServerSpecification:
                            let serverSpec = NetworkMessageObjectGeneric<ServerSpecificationObject>.init(json: json)
                            
                            guard let message = serverSpec.message() else {
                                _connectionStatus = .Disconnected
                                break
                            }
                            guard let serverPort = message.serverPort() else {
                                _connectionStatus = .Disconnected
                                break
                            }
                            
                            _serverPort = serverPort
                            _connectionStatus = .Connected
                            break;
                
                        default:
                            _connectionStatus = .Disconnected
                    }
                }
            } else {
                _connectionStatus = .Disconnected
            }
        } catch {
            _connectionStatus = .Disconnected
        }
    }
    
    func attemptConnection() {
        let discoveryObject = NetworkDiscoveryObject(identifier: "Photo.Management.Studio", clientSocketPort: _serverPort)
        _socketOut!.send(discoveryObject.toJSON());
    }
    
    func pingServer() {
        print("pingServer...")
        
        let url = "http://\(_imageServerAddress):\(_imageServerPort)/api/ping"
        print(url)
        
        /*
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

        */
    }
}

class InSocket : NSObject, GCDAsyncSocketDelegate {
    var _port: UInt16

    var _socket: GCDAsyncSocket!
    var _delegate: ServerInfoReceivedDelegate;
    
    var _connectedSockets: Array<GCDAsyncSocket>
    
    init(delegate: ServerInfoReceivedDelegate, port: UInt16) {
        _delegate = delegate;
        _port = port;
        
        _connectedSockets = Array<GCDAsyncSocket>()
        
        super.init()
        setupConnection()
    }
    
    func getPort() -> UInt16 {
        return _port
    }
    
    func setupConnection() {
        
        if (_port == 0) {
            let port = PortHelper.availableTcpPort()
            if (port == -1) {
                return
            }
            _port = UInt16(port)
        }
        
        _socket = GCDAsyncSocket(delegate: self, delegateQueue: dispatch_get_main_queue())
        do {
            try _socket.acceptOnPort(_port)
        } catch {
            print("Something went wrong")
        }
    }
    
    func socket(sock: GCDAsyncSocket!, didAcceptNewSocket newSocket: GCDAsyncSocket!) {
        print("Accepted new socket from \(newSocket.connectedHost):\(newSocket.connectedPort)")
        _connectedSockets.append(newSocket)
        
        let data = "OK".dataUsingEncoding(NSUTF8StringEncoding)
        newSocket.writeData(data, withTimeout: 2, tag: 0)
        newSocket.readDataWithTimeout(-1, tag: 0)
    }
    
    func socket(sock: GCDAsyncSocket!, didConnectToHost host: String!, port: UInt16) {
        print("did connect to host: \(host)")
    }
    
    func socket(sock: GCDAsyncSocket!, didReadData data: NSData!, withTag tag: Int) {
        
        //Yes, I know this wastefully allocates RAM, so I'll fix it later.
        if var string = NSString(data: data, encoding: NSASCIIStringEncoding)?
            .stringByReplacingOccurrencesOfString("\t", withString: "")
            .stringByReplacingOccurrencesOfString("\0", withString: "") {
        
            if string.hasSuffix("<EOF>") {
                string = string.substringToIndex(string.endIndex.advancedBy(-5))
            }
        
            print(string)

            if let cleanData = string.dataUsingEncoding(NSUTF8StringEncoding) {
                _delegate.onServerInfoReceived(cleanData)
            }
        }
        
        sock.disconnectAfterReadingAndWriting()
        if let idx = _connectedSockets.indexOf(sock) {
            _connectedSockets.removeAtIndex(idx)
        }
    }
    
    
//    func udpSocket(sock: GCDAsyncUdpSocket!, didReceiveData data: NSData!, fromAddress address: NSData!, withFilterContext filterContext: AnyObject!) {
//        print("incoming message: \(data)")
        
//        _delegate.onServerInfoChanged("Hello World", port: 5678)
//    }
}

class OutSocket : NSObject, GCDAsyncUdpSocketDelegate {
    let _ipAddress: String
    let _port: UInt16
    var _socket:GCDAsyncUdpSocket!
    let _ipAddressData: NSData
    
    init(ipAddress: String, port: UInt16) {
        _ipAddress = ipAddress
        _port = port
        
        var ip = sockaddr_in()
        ip.sin_family = sa_family_t(AF_INET)
        ip.sin_port = _port.littleEndian
        inet_pton(AF_INET, _ipAddress, &ip.sin_addr)
        
        _ipAddressData = NSData.init(bytes: &ip, length: sizeofValue(ip))
        
        super.init()
        setupConnection()
    }
    
    func setupConnection() {
        _socket = GCDAsyncUdpSocket(delegate: self, delegateQueue: dispatch_get_main_queue())
        _socket.setPreferIPv4()
        do {
            try _socket.enableBroadcast(true)
        } catch {
            print("enableBroadcast failed")
        }
    }
    
    func send(message: [String: AnyObject]) {
        do {
            let data = try NSJSONSerialization.dataWithJSONObject(message, options: NSJSONWritingOptions(rawValue: 0))
            _socket.sendData(data, toHost: _ipAddress, port: _port, withTimeout: -1, tag: 0)
        } catch {
            print("send: something went wrong")
        }
    }
    
    func udpSocket(sock: GCDAsyncUdpSocket!, didConnectToAddress address: NSData!) {
        print("didConnectToAddress")
    }
    
    func udpSocket(sock: GCDAsyncUdpSocket!, didNotConnect error: NSError!) {
        print("didNotConnect \(error)")
    }
    
    func udpSocket(sock: GCDAsyncUdpSocket!, didSendDataWithTag tag: Int) {
        print("didSendDataWithTag")
    }
    
    func udpSocket(sock: GCDAsyncUdpSocket!, didNotSendDataWithTag tag: Int, dueToError error: NSError!) {
        print("didNotSendDataWithTag \(error)")
    }
}


/*

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

*/