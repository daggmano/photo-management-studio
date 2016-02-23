//
//  ViewController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 6/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class MainViewController: NSViewController {
    
    @IBOutlet var statusLabel: NSTextField!

    override func viewDidLoad() {
        super.viewDidLoad()
        
        Event.register("connection-status-changed") { status -> Void in
            print("Hey, here is \(status)")
            
            dispatch_async(dispatch_get_main_queue(), { () -> Void in
                self.statusLabel.stringValue = status as! String
            })
            
        }

        // Do any additional setup after loading the view.
    }

    override var representedObject: AnyObject? {
        didSet {
        // Update the view, if already loaded.
        }
    }


}

