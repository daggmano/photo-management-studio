//
//  AppDelegate.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 18/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa
import Fabric
import Crashlytics

@NSApplicationMain
class AppDelegate: NSObject, NSApplicationDelegate, NetworkConnectionStatusDelegate {

    var _networkSupervisor: NetworkSupervisor!
    var _connectionStatus: ConnectionState!
    var _remoteDatabaseAddress: String?
    var _serverUrl: String?

    func applicationDidFinishLaunching(aNotification: NSNotification) {
        // Insert code here to initialize your application
        
        NSUserDefaults.standardUserDefaults().registerDefaults(["NSApplicationCrashOnExceptions": true])
        Fabric.with([Crashlytics.self])
        
        _networkSupervisor = NetworkSupervisor(delegate: self)

    }
    
    static func getInstance() -> AppDelegate? {
        return NSApplication.sharedApplication().delegate as? AppDelegate
    }

    func applicationWillTerminate(aNotification: NSNotification) {
        // Insert code here to tear down your application
    }

    func onServerConnectionStatusChanged(status: ConnectionState) {
        print("new status: \(status)")
        self._connectionStatus = status
        Event.emit("connection-status-changed", obj: status.description)
    }
    
    func getConnectionStatus() -> ConnectionState {
        return _connectionStatus
    }
    
    func setRemoteDatabaseAddress(url: String) {
        self._remoteDatabaseAddress = url
        Event.emit("connection-status-changed", obj: _connectionStatus.description)
    }
    
    func getRemoteDatabaseAddress() -> String? {
        if let status = self._connectionStatus {
            if status == .Connected {
                return _remoteDatabaseAddress
            }
        }
        return nil
    }
    
    func setServerUrl(url: String) {
        self._serverUrl = url
        Event.emit("connection-status-changed", obj: _connectionStatus.description)
    }
    
    func getServerUrl() -> String? {
        if let status = self._connectionStatus {
            if status == .Connected {
                return _serverUrl
            }
        }
        return nil
    }
}

