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
class AppDelegate: NSObject, NSApplicationDelegate {

    func applicationDidFinishLaunching(aNotification: NSNotification) {
        // Insert code here to initialize your application
        
        NSUserDefaults.standardUserDefaults().registerDefaults(["NSApplicationCrashOnExceptions": true])
        Fabric.with([Crashlytics.self])
    }

    func applicationWillTerminate(aNotification: NSNotification) {
        // Insert code here to tear down your application
    }


}

