//
//  NetworkSupervisor.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 31/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Alamofire
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

class NetworkSupervisor: NSObject, ServerInfoReceivedDelegate {
    
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
        
        _imageServerPort = 0
        _imageServerAddress = ""
        
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
            
            self._watchdogTimer = NSTimer.scheduledTimerWithTimeInterval(Double(timeout) / 1000.0, target: self, selector: #selector(NetworkSupervisor.onWatchdogTimer(_:)), userInfo: nil, repeats: repeats)
        }
    }
    
    func onWatchdogTimer(sender: NSTimer!) {
        switch (_connectionStatus) {
        case .Disconnected:
            print(".Disconnected")
            attemptConnection()
            break
            
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
        
        Alamofire.request(.GET, url).responseJSON { response in
            if response.result.isFailure {
                self._connectionStatus = .Disconnected
                self.setupWatchdog(500, repeats: true)
                self._delegate.onServerConnectionStatusChanged(self._connectionStatus)
            } else if response.result.isSuccess {
                if let json = response.result.value as? [String: AnyObject] {
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
                }
            }
        }
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
