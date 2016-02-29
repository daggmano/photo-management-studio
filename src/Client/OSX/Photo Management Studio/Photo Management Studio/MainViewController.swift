//
//  ViewController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 6/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class MainViewController: NSTabViewController {
    
    @IBOutlet var statusLabel: NSTextField!

    override func viewDidLoad() {
        super.viewDidLoad()
        
        

        // Do any additional setup after loading the view.
    }
    
    override func viewDidAppear() {
        super.viewDidAppear()
        
        self.view.window?.titleVisibility = .Hidden
        self.view.window?.titlebarAppearsTransparent = true
        self.view.window?.movableByWindowBackground = true
    }

    @IBAction func importPhotos(sender: AnyObject?) {
        self.tabView.selectTabViewItemAtIndex(1)
    }
    
    override var representedObject: AnyObject? {
        didSet {
        // Update the view, if already loaded.
        }
    }


}

