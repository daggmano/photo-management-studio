//
//  MainWindowController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 29/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class MainWindowController : NSWindowController {
    
    @IBOutlet weak var statusIcon: NSToolbarItem!
    
    override func windowDidLoad() {
        super.windowDidLoad()
        
        Event.register("connection-status-changed") { status -> Void in
            print("Hey, here is \(status)")
            
            dispatch_async(dispatch_get_main_queue(), { () -> Void in
                if let status = status as? String {
                    switch (status) {
                    case "Disconnected":
                        self.statusIcon.image = NSImage(named: "NSStatusUnavailable")
                        break
                    case "Connecting":
                        self.statusIcon.image = NSImage(named: "NSStatusPartiallyAvailable")
                        break
                    case "Connected":
                        self.statusIcon.image = NSImage(named: "NSStatusAvailable")
                        break
                    default:
                        break;
                    }
                }
            })
        }
    }
}
