//
//  ImportView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 3/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class ImportView: NSView {

    override func drawRect(dirtyRect: NSRect) {
        
        NSColor(deviceWhite: 69.0/255.0, alpha: 1.0).setFill()
        NSRectFill(dirtyRect)
        
        super.drawRect(dirtyRect)
        
        // Drawing code here.
    }
    
}
