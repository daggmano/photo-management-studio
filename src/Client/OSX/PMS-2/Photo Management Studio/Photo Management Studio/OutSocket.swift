//
//  OutSocket.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 31/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import CocoaAsyncSocket

class OutSocket: NSObject, GCDAsyncUdpSocketDelegate {
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
