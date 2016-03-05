//
//  NetworkSupervisor.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 6/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation
import CocoaAsyncSocket

enum ConnectionState: String {
    var description: String { return self.rawValue }
    
    case Disconnected = "Disconnected"
    case Connecting = "Connecting"
    case Connected = "Connected"
}

protocol ServerInfoReceivedDelegate {
    func onServerInfoReceived(message: NSData)
    func onDbServerInfoReceived(message: ServerInfoResponseObject)
}

protocol NetworkConnectionStatusDelegate {
    func onServerConnectionStatusChanged(status: ConnectionState)
    func setServerUrl(url: String)
}

class NetworkSupervisor : NSObject, ServerInfoReceivedDelegate {
    
    var _delegate: NetworkConnectionStatusDelegate
    
    var _connectionStatus: ConnectionState
    var _watchdogTimer: NSTimer?
    
    var _socketIn: InSocket!
    var _socketOut: OutSocket!
    
    var _serverPort: UInt16!
    
    var _imageServerAddress: String!
    var _imageServerPort: UInt16!
    
    required init(delegate: NetworkConnectionStatusDelegate) {
        _delegate = delegate
        
        _connectionStatus = .Disconnected
        _delegate.onServerConnectionStatusChanged(_connectionStatus)

        _imageServerAddress = ""
        _imageServerPort = 0

        super.init()

        _socketIn = InSocket.init(delegate: self, port: 0)
        _serverPort = _socketIn.getPort()
        NSLog("Server Port is \(_serverPort)")
        _socketOut = OutSocket.init(ipAddress: "255.255.255.255", port: Config.udpSearchPort)
        
        setupWatchdog(500, repeats: true)
    }
    
    private func setupWatchdog(timeout: Int, repeats: Bool) {
        dispatch_async(dispatch_get_main_queue()) { [unowned self] in
            if let watchdogTimer = self._watchdogTimer {
                watchdogTimer.invalidate()
            }
            
            self._watchdogTimer = NSTimer.scheduledTimerWithTimeInterval(Double(timeout) / 1000.0, target: self, selector: "onWatchdogTimer:", userInfo: nil, repeats: repeats)
        }
    }
    
    func onWatchdogTimer(sender: NSTimer!) {
        switch (_connectionStatus)
        {
        case .Disconnected:
            print(".Disconnected")
            attemptConnection()
            break;
            
        case .Connecting:
            print(".Connecting")
            pingServer()
            break
            
        case .Connected:
            print(".Connected")
            pingServer()
            break
        }
    }
    
    func onServerInfoReceived(message: NSData) {
        do {
            if let json = try NSJSONSerialization.JSONObjectWithData(message, options: .AllowFragments) as? [String: AnyObject] {
                let networkMessage = NetworkMessageObject.init(json: json)
            
                if let messageType = networkMessage.messageType {
                    switch (messageType) {
                        case .ServerSpecification:
                            let serverSpec = NetworkMessageObjectGeneric<ServerSpecificationObject>.init(json: json)
                            
                            guard let message = serverSpec.message,
                                let serverAddress = message.serverAddress,
                                let serverPort = message.serverPort else {
                                break
                            }
                            
                            _imageServerAddress = serverAddress
                            _imageServerPort = serverPort
                            _connectionStatus = .Connecting
                            _delegate.onServerConnectionStatusChanged(_connectionStatus)
                            break
                    }
                }
            }
        } catch {}
    }
    
    func onDbServerInfoReceived(message: ServerInfoResponseObject) {
        print(message.data?.serverId)
        print(message.data?.serverName)
        
        if let serverId = message.data?.serverId {
            DatabaseManager.setupReplication(_imageServerAddress, serverId: serverId)
            _connectionStatus = .Connected
            _delegate.onServerConnectionStatusChanged(_connectionStatus)
            _delegate.setServerUrl("http://\(_imageServerAddress):\(_imageServerPort)")
        }
    }
    
    func attemptConnection() {
        let discoveryObject = NetworkDiscoveryObject(identifier: "Photo.Management.Studio", clientSocketPort: _serverPort)
        _socketOut!.send(discoveryObject.toJSON());
    }
    
    func pingServer() {
        print("pingServer...")
        
        let url = "http://\(_imageServerAddress):\(_imageServerPort)/api/ping"
        
        let request = NSMutableURLRequest(URL: NSURL(string: url)!)
        let session = NSURLSession.sharedSession()
        
        let task = session.dataTaskWithRequest(request, completionHandler: {(data: NSData?, response: NSURLResponse?, error: NSError?) -> Void in
            do {
                if let data = data {
                    if let json = try NSJSONSerialization.JSONObjectWithData(data, options: .AllowFragments) as? [String: AnyObject] {
                        let pingResponse = PingResponseObject(json: json)

                        if let responseData = pingResponse.data {
                            print(responseData.serverDateTime)
                            
                            if (self._connectionStatus == .Connecting) {
                                self.getDbServerId()
                                self.setupWatchdog(1000, repeats: false)
                            } else {
                                self.setupWatchdog(5000, repeats: false)
                            }
                        } else {
                            self._connectionStatus = .Disconnected
                            self.setupWatchdog(500, repeats: true)
                            self._delegate.onServerConnectionStatusChanged(self._connectionStatus)
                            print("unable to get responseData")
                        }
                    } else {
                        self._connectionStatus = .Disconnected
                        self.setupWatchdog(500, repeats: true)
                        self._delegate.onServerConnectionStatusChanged(self._connectionStatus)
                        print("unable to decode responseObject")
                    }
                } else {
                    self._connectionStatus = .Disconnected
                    self.setupWatchdog(500, repeats: true)
                    self._delegate.onServerConnectionStatusChanged(self._connectionStatus)
                    print("ping response data is null")
                }
            } catch {
                self._connectionStatus = .Disconnected
                self.setupWatchdog(500, repeats: true)
                self._delegate.onServerConnectionStatusChanged(self._connectionStatus)
            }
        })
        task.resume()
    }
    
    func getDbServerId() {
        print("getDbServerId...")
        
        let url = "http://\(_imageServerAddress):\(_imageServerPort)/api/serverInfo"
        
        let request = NSMutableURLRequest(URL: NSURL(string: url)!)
        let session = NSURLSession.sharedSession()
        
        let task = session.dataTaskWithRequest(request, completionHandler: {(data: NSData?, response: NSURLResponse?, error: NSError?) -> Void in
            do {
                if let data = data {
                    if let json = try NSJSONSerialization.JSONObjectWithData(data, options: .AllowFragments) as? [String: AnyObject] {
                        let serverInfoResponse = ServerInfoResponseObject(json: json)
                        self.onDbServerInfoReceived(serverInfoResponse)
                    }
                }
            } catch {}
        })
        task.resume()
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
