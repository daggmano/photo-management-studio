//
//  ImportViewItemView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 29/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class ImportViewItemView: NSView {
    
    @IBOutlet weak var innerView: NSView!
    
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
        self.innerView.layer!.cornerRadius = 5
        self.innerView.layer!.borderWidth = 2
        
        if selected {
            self.innerView.layer!.backgroundColor = NSColor(white: 0.3, alpha: 1.0).CGColor
        } else {
            self.innerView.layer!.backgroundColor = NSColor.clearColor().CGColor
        }
        
        if highlightState == .AsDropTarget {
            self.innerView.layer!.borderColor = NSColor(deviceRed: 0, green: 0.8, blue: 1.0, alpha: 1.0).CGColor
        } else {
            self.innerView.layer!.borderColor = NSColor.clearColor().CGColor
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
