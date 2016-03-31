//
//  InSocket.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 31/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import CocoaAsyncSocket

class InSocket: NSObject, GCDAsyncSocketDelegate {
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
