//
//  DividerView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 28/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class DividerView: NSView {

    override func drawRect(dirtyRect: NSRect) {
        
        NSColor(white: 0.15, alpha: 1.0).setFill()
        NSRectFill(dirtyRect)
        
        super.drawRect(dirtyRect)

        // Drawing code here.
    }
    
}
