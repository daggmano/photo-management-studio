//
//  PMSBox.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 17/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class PMSBox: NSBox {

    override func drawRect(dirtyRect: NSRect) {
        //super.drawRect(dirtyRect)

        // Drawing code here.
        NSColor(red: 1.0, green: 1.0, blue: 1.0, alpha: 1.0).set()
        super.drawRect(dirtyRect)
//        NSRectFill(dirtyRect)
        
    }
    
}
