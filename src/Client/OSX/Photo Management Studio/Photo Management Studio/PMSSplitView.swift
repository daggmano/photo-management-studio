//
//  PMSSplitView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 16/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class PMSSplitView: NSSplitView {

    override func drawRect(dirtyRect: NSRect) {
        let topView = self.subviews[0]
        let topViewFrameRect = topView.frame
        
        let rect = self.vertical
            ? NSMakeRect(topViewFrameRect.size.width, topViewFrameRect.origin.y, self.dividerThickness, topViewFrameRect.size.height)
            : NSMakeRect(topViewFrameRect.origin.x, topViewFrameRect.size.height, topViewFrameRect.size.width, self.dividerThickness)
        
        self.drawDividerInRect(rect)
    }
    
    override func drawDividerInRect(rect: NSRect) {
        NSColor(calibratedWhite: 0.3, alpha: 1.0).set()
        
        NSRectFill(rect)
    }
}
