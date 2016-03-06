//
//  PhotoViewItem.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 2/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class PhotoViewItem: NSCollectionViewItem {
    
    private func updateView() {
        let uuid = NSUUID().UUIDString
        
        super.viewWillAppear()
        
        self.imageView?.image = NSImage(named: "placeholder")
        self.label.stringValue = ""
        
        if let item = self.importableItem {
            
            if let filename = item.filename {
                self.label.stringValue = filename
            }
            
            if let thumbUrl = item.thumbUrl {
                let url = "\(thumbUrl)&size=500"
                    
                dispatch_async(dispatch_queue_create("getAsyncPhotosGDQueue", nil), { () -> Void in
                    if let url = NSURL(string: url) {
                        if let image = NSImage(contentsOfURL: url) {
                            dispatch_async(dispatch_get_main_queue(), { () -> Void in
                                self.imageView?.image = image
                                self.imageView?.needsDisplay = true
                            })
                        }
                    }
                })
            }
        }
    }
    
    // MARK: properties
    
    var importableItem: ImportableItem? {
        return representedObject as? ImportableItem
    }
    
    override var representedObject: AnyObject? {
        didSet {
            super.representedObject = representedObject

            if let _ = representedObject as? ImportableItem {
                self.updateView()
            }
        }
    }
    
    override var selected: Bool {
        didSet {
            (self.view as! PhotoViewItemView).selected = selected
        }
    }
    
    override var highlightState: NSCollectionViewItemHighlightState {
        didSet {
            (self.view as! PhotoViewItemView).highlightState = highlightState
        }
    }
    
    // MARK: outlets
    
    @IBOutlet weak var label: NSTextField!
    
    // MARK: NSResponder
    
    override func mouseDown(theEvent: NSEvent) {
        if theEvent.clickCount == 2 {
            print("Double click \(importableItem!.fullPath)")
        } else {
            super.mouseDown(theEvent)
        }
    }
}
