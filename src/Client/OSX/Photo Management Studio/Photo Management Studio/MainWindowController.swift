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
    
    var _homeViewController: HomeViewController!
    var _importViewController: ImportViewController!
    
    var _previewWindowController: PreviewWindowController!
    
    override func windowDidLoad() {
        super.windowDidLoad()
        
        self.window?.titleVisibility = .Hidden
        self.window?.titlebarAppearsTransparent = true
        self.window?.movableByWindowBackground = true
        
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
        
        Event.register("display-preview") { (imageUrl) -> Void in
            if let imageUrl = imageUrl as? String {
                dispatch_async(dispatch_get_main_queue(), { () -> Void in
                    if (self._previewWindowController == nil) {
                        self._previewWindowController = PreviewWindowController(windowNibName: "PreviewWindow")
                    }
                
                    self._previewWindowController.setImageUrl(imageUrl)
                    self._previewWindowController.showWindow(self)
                })
            }
        }
        
        _homeViewController = HomeViewController(nibName: "HomeView", bundle: nil)
        _homeViewController.view.autoresizingMask = [.ViewHeightSizable, .ViewWidthSizable]
        self.window?.contentView?.addSubview(_homeViewController.view)
        _homeViewController.view.frame = (self.window?.contentView?.bounds)!
    }
    
    @IBAction func importPhotos(sender: AnyObject?) {
        if (_importViewController == nil) {
            _importViewController = ImportViewController(nibName: "ImportView", bundle: nil)
            _importViewController.view.autoresizingMask = [.ViewHeightSizable, .ViewWidthSizable]
        }
        
        for view: NSView in (self.window?.contentView?.subviews)! {
            view.removeFromSuperview()
        }
        
        self.window?.contentView?.addSubview(_importViewController.view)
        _importViewController.view.frame = (self.window?.contentView?.bounds)!
    }
    
    @IBAction func goHome(sender: AnyObject?) {
        if (_homeViewController == nil) {
            _homeViewController = HomeViewController(nibName: "HomeView", bundle: nil)
            _homeViewController.view.autoresizingMask = [.ViewHeightSizable, .ViewWidthSizable]
        }
        
        for view: NSView in (self.window?.contentView?.subviews)! {
            view.removeFromSuperview()
        }
        
        self.window?.contentView?.addSubview(_homeViewController.view)
        _homeViewController.view.frame = (self.window?.contentView?.bounds)!
    }
}
