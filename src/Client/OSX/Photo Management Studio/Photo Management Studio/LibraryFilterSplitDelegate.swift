//
//  LibraryFilterSplitDelegate.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 16/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class LibraryFilterSplitDelegate: NSObject, NSSplitViewDelegate {

    func splitView(splitView: NSSplitView, effectiveRect proposedEffectiveRect: NSRect, forDrawnRect drawnRect: NSRect, ofDividerAtIndex dividerIndex: Int) -> NSRect {
        return NSZeroRect
    }
    
    func splitView(splitView: NSSplitView, constrainMaxCoordinate proposedMaximumPosition: CGFloat, ofSubviewAt dividerIndex: Int) -> CGFloat {
        return 40.0
    }
    
    func splitView(splitView: NSSplitView, constrainMinCoordinate proposedMinimumPosition: CGFloat, ofSubviewAt dividerIndex: Int) -> CGFloat {
        return 40.0
    }
    
    func splitView(splitView: NSSplitView, shouldAdjustSizeOfSubview view: NSView) -> Bool {
        if view == splitView.subviews[0] {
            return false
        }
        return true
    }
}
