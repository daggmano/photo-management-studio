//
//  TitleBarRightViewController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 18/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class TitleBarRightViewController: NSViewController {

    @IBOutlet weak var statusIcon: NSImageView!
    
    required init?(coder: NSCoder) {
        super.init(coder: coder)
    }
    
    override init?(nibName nibNameOrNil: String?, bundle nibBundleOrNil: NSBundle?) {
        super.init(nibName: nibNameOrNil, bundle: nibBundleOrNil)
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        // Do view setup here.

        self.view.layer?.backgroundColor = CGColorCreateGenericGray(1.0, 1.0)
        
        Event.register("connection-status-changed") { (status) -> Void in
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
