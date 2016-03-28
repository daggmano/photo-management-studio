//
//  TitleBarLeftView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 19/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class TitleBarLeftView: NSView {

    override func drawRect(dirtyRect: NSRect) {
        
        NSColor(deviceRed: 49.0/255.0, green: 49.0/255.0, blue: 49.0/255.0, alpha: 1.0).setFill()
        NSRectFill(dirtyRect)
        
        super.drawRect(dirtyRect)

        // Drawing code here.
    }
    
}
