//
//  ImportItemViewItemView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 2/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class ImportViewItemView: NSView {
    
    @IBOutlet var innerView: NSView!

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
            self.innerView.layer!.backgroundColor = NSColor(red: 0.3, green: 0.3, blue: 0.3, alpha: 1.0).CGColor
        } else {
            self.innerView.layer!.backgroundColor = NSColor(red: 0.2, green: 0.2, blue: 0.2, alpha: 1.0).CGColor
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
