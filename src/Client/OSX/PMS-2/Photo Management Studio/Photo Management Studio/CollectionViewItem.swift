//
//  CollectionViewItem.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 28/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class CollectionViewItem: NSCollectionViewItem {
    
    @IBOutlet weak var titleLabel: NSTextField!
    @IBOutlet weak var subTitleLabel: NSTextField!
    
    private func updateView() {
        super.viewWillAppear()
        
        self.imageView?.image = NSImage(named: "placeholder")
        titleLabel.stringValue = ""
        subTitleLabel.stringValue = ""
        view.toolTip = ""
        
        if let item = self.importableItem {
            
            titleLabel.stringValue = item.title

            if let subTitle = item.subTitle {
                subTitleLabel.stringValue = subTitle
                self.view.toolTip = "\(item.title)\n\n\(subTitle)"
            } else {
                subTitleLabel.stringValue = ""
                self.view.toolTip = item.title
            }
            
            if !item.imageUrl.containsString("http:") {
                imageView?.image = NSImage(named: item.imageUrl)
            } else {
                let url = "\(item.imageUrl)&size=500"
                    
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
