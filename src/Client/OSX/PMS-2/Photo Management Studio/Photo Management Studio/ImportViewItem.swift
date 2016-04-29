//
//  ImportViewItem.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 29/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class ImportViewItem: NSCollectionViewItem {
    
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
                var url: String;
                if (item.imageUrl.containsString("?")) {
                    url = "\(item.imageUrl)&size=500"
                } else {
                    url = "\(item.imageUrl)?size=500"
                }
                print(url)
                let thumbName = "\(item.identifier!)_500x500.jpg"
                
                ImageCache.getImage(url, thumbName: thumbName, useCache: true, callback: { (image) in
                    self.imageView?.image = image
                    self.imageView?.needsDisplay = true
                })
            }
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
            (self.view as! ImportViewItemView).selected = selected
        }
    }
    
    override var highlightState: NSCollectionViewItemHighlightState {
        didSet {
            (self.view as! ImportViewItemView).highlightState = highlightState
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
