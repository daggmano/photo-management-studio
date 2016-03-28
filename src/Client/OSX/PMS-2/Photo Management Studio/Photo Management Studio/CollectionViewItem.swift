//
//  CollectionViewItem.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 28/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class CollectionViewItem: NSCollectionViewItem {
    
    @IBOutlet weak var nameLabel: NSTextField!
    @IBOutlet weak var sizeLabel: NSTextField!

//    override func viewWillAppear() {
//        if let photoItem = representedObject as? PhotoItem {
//            nameLabel.stringValue = photoItem.fileName
//            sizeLabel.stringValue = photoItem.dimensions
//            imageView?.image = NSImage(named: photoItem.imageUrl)
//        }
//    }
    
    
    private func updateView() {
        super.viewWillAppear()
        
//        self.imageView?.image = NSImage(named: "placeholder")
        nameLabel.stringValue = ""
        sizeLabel.stringValue = ""
        
        if let item = self.importableItem {
            
            if let fileName = item.fileName {
                nameLabel.stringValue = fileName
            }
            if let dimensions = item.dimensions {
                sizeLabel.stringValue = dimensions
            }
            
            if let imageUrl = item.imageUrl {
                imageView?.image = NSImage(named: imageUrl)
            }
            
//            if let thumbUrl = item.thumbUrl {
//                let url = "\(thumbUrl)&size=500"
                
//                dispatch_async(dispatch_queue_create("getAsyncPhotosGDQueue", nil), { () -> Void in
//                    if let url = NSURL(string: url) {
//                        if let image = NSImage(contentsOfURL: url) {
//                            dispatch_async(dispatch_get_main_queue(), { () -> Void in
//                                self.imageView?.image = image
//                                self.imageView?.needsDisplay = true
//                            })
//                        }
//                    }
//                })
//            }
        }
    }
    
    // MARK: properties
    
    var importableItem: PhotoItem? {
        return representedObject as? PhotoItem
    }
    
    override var representedObject: AnyObject? {
        didSet {
            super.representedObject = representedObject
            
            if let _ = representedObject as? PhotoItem {
                self.updateView()
            }
        }
    }
    
    override var selected: Bool {
        didSet {
            (self.view as! CollectionViewItemView).selected = selected
        }
    }
    
    override var highlightState: NSCollectionViewItemHighlightState {
        didSet {
            (self.view as! CollectionViewItemView).highlightState = highlightState
        }
    }
    
    // MARK: NSResponder
    
    override func mouseDown(theEvent: NSEvent) {
        if theEvent.clickCount == 2 {
//            if let thumbUrl = importableItem?.thumbUrl {
//                print("Double click \(importableItem!.fullPath)")
                
//                Event.emit("display-preview", obj: thumbUrl)
//            }
        } else {
            super.mouseDown(theEvent)
        }
    }
}
