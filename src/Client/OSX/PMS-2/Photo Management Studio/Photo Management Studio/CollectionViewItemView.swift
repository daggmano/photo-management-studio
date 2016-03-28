//
//  CollectionViewItemView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 28/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class CollectionViewItemView: NSView {

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
        
        if selected {
            self.innerView.layer!.backgroundColor = NSColor(white: 0.3, alpha: 1.0).CGColor
        } else {
            self.innerView.layer!.backgroundColor = NSColor.clearColor().CGColor
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
