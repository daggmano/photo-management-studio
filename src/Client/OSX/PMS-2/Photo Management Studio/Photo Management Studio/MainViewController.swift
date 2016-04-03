//
//  ViewController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 18/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class MainViewController: NSSplitViewController {
    
    override func viewDidLoad() {
        
        if let splitView = view as? NSSplitView {
            splitView.setPosition(200, ofDividerAtIndex: 0)
        }
    }

    override func splitView(splitView: NSSplitView, effectiveRect proposedEffectiveRect: NSRect, forDrawnRect drawnRect: NSRect, ofDividerAtIndex dividerIndex: Int) -> NSRect {
        return NSZeroRect
    }
}
