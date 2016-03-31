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

    func applicationDidFinishLaunching(aNotification: NSNotification) {
        // Insert code here to initialize your application
        
        NSUserDefaults.standardUserDefaults().registerDefaults(["NSApplicationCrashOnExceptions": true])
        Fabric.with([Crashlytics.self])
        
        _networkSupervisor = NetworkSupervisor(delegate: self)

    }

    func applicationWillTerminate(aNotification: NSNotification) {
        // Insert code here to tear down your application
    }

    func onServerConnectionStatusChanged(status: ConnectionState) {
        print("new status: \(status)")
//        self._connectionStatus = status
//        Event.emit("connection-status-changed", obj: status.description)
    }

    func setServerUrl(url: String) {
//        self._serverUrl = url
    }
    
    func getServerUrl() -> String? {
//        if (self._connectionStatus == .Connected) {
//            return _serverUrl
//        }
        return nil
    }

}

