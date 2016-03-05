//
//  PhotoItemViewItemView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 2/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class PhotoViewItemView: NSView {

    // MARK: properties
    
    var selected: Bool = false {
        didSet {
            if selected != oldValue {
                needsDisplay = true
            }
        }
    }
    
    var highlightState: NSCollectionViewItemHighlightState = .None {
        didSet {
            if highlightState != oldValue {
                needsDisplay = true
            }
        }
    }
    
    // MARK: NSView
    
    override var wantsUpdateLayer: Bool {
        return true
    }
    
    override func updateLayer() {
        if selected {
//            self.layer?.cornerRadius = 10
            layer!.backgroundColor = NSColor(red: 0.5, green: 0.5, blue: 0.8, alpha: 1.0).CGColor
        } else {
//            self.layer?.cornerRadius = 10
            layer!.backgroundColor = NSColor.lightGrayColor().CGColor
        }
    }
    
    // MARK: init
    
    override init(frame frameRect: NSRect) {
        super.init(frame: frameRect)
        wantsLayer = true
        layer?.masksToBounds = true
    }
    
    required init?(coder: NSCoder) {
        super.init(coder: coder)
        wantsLayer = true
        layer?.masksToBounds = true
    }
}
