//
//  TitleBarSplitView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 18/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class TitleBarSplitView: SplitView {
    
    override var mouseDownCanMoveWindow: Bool {
        return false
    }
    
    override func mouseDown(theEvent: NSEvent) {
        
        guard let window = self.window else {
            return
        }
        
        var mouseLocation = theEvent.locationInWindow
        
        var dividerRect = NSMakeRect(NSMaxX(self.subviews[0].frame), NSMinY(self.bounds), self.dividerThickness, NSHeight(self.bounds))
        dividerRect = NSInsetRect(dividerRect, -2, 0)
            
        let mouseLocationInMyCoords = self.convertPoint(mouseLocation, fromView: nil)
        if (!self.mouse(mouseLocationInMyCoords, inRect: dividerRect)) {
            mouseLocation = window.convertPointToScreen(mouseLocation)
            var origin = window.frame.origin
                
            // Now we loop handling mouse events unwil we get a mouse up event.
            while ((theEvent == NSApp.nextEventMatchingMask(Int(NSEventMask.LeftMouseDownMask.rawValue | NSEventMask.LeftMouseDraggedMask.rawValue | NSEventMask.LeftMouseUpMask.rawValue), untilDate: NSDate.distantFuture(), inMode: NSEventTrackingRunLoopMode, dequeue: true) && (theEvent.type != .LeftMouseUp))) {
                
                let currentLocation = window.convertPointToScreen(theEvent.locationInWindow)
                origin.x += currentLocation.x - mouseLocation.x
                origin.y += currentLocation.y - mouseLocation.y
                    
                // Move the window by the mouse displacement since the last event.
                window.setFrameOrigin(origin)
                mouseLocation = currentLocation
            }
                
            super.mouseUp(theEvent)
            return
        }
        
        super.mouseDown(theEvent)
    }
}

extension NSWindow {
    func convertPointToScreen(point: NSPoint) -> NSPoint {
        let convertRect = self.convertRectToScreen(NSMakeRect(point.x, point.y, 0.0, 0.0))
        return NSMakePoint(convertRect.origin.x, convertRect.origin.y);
    }
}
