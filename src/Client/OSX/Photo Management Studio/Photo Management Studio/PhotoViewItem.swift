//
//  PhotoViewItem.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 2/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class PhotoViewItem: NSCollectionViewItem {
    
    override func viewWillAppear() {
        super.viewWillAppear()
        
        if let item = importableItem {
            if let thumbUrl = item.thumbUrl {
                let url = "\(thumbUrl)&size=500"
                self.imageView?.image = NSImage(contentsOfURL: NSURL(string: url)!)
            }
        }
    }

    // MARK: properties
    
    var importableItem: ImportableItem? {
        return representedObject as? ImportableItem
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
    
//    @IBOutlet weak var image: NSImage!
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
