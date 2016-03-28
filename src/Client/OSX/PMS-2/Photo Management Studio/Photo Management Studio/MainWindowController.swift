//
//  MainWindowController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 18/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa
import INAppStoreWindow

class MainWindowController: NSWindowController, NSSplitViewDelegate {
    
    var _inAppStoreWindow: INAppStoreWindow!
    
    required init?(coder: NSCoder) {
        _updatingLinkedSplitView = false

        super.init(coder: coder)
    }
    
    override init(window: NSWindow?) {
        _updatingLinkedSplitView = false
        
        super.init(window: window)
    }

    var _windowTitleBarView: TitleBarSplitView!
    
    var _titleBarLeftViewController: TitleBarLeftViewController!
    var _titleBarRightViewController: TitleBarRightViewController!
    
    var _updatingLinkedSplitView: Bool
    var _linkedSplitView: NSSplitView?
    
    override func windowDidLoad() {
        super.windowDidLoad()
        
        window?.contentView?.superview?.wantsLayer = true
        
        _updatingLinkedSplitView = false
        
        _inAppStoreWindow = self.window as? INAppStoreWindow
        _inAppStoreWindow.opaque = false
        
        _windowTitleBarView = TitleBarSplitView()
        _windowTitleBarView.setHoldingPriority(1, forSubviewAtIndex: 1)
        _windowTitleBarView.setPosition(200.0, ofDividerAtIndex: 0)
        _windowTitleBarView.dividerStyle = .Thin
        _windowTitleBarView.vertical = true
        _windowTitleBarView.frame = (_inAppStoreWindow?.titleBarView.bounds)!
        _windowTitleBarView.autoresizingMask = [.ViewHeightSizable, .ViewWidthSizable]
        _windowTitleBarView.delegate = self
        
        _titleBarLeftViewController = TitleBarLeftViewController(nibName: "TitleBarLeftView", bundle: nil)
        _titleBarLeftViewController.view.autoresizingMask = [.ViewHeightSizable, .ViewWidthSizable]

        _titleBarRightViewController = TitleBarRightViewController(nibName: "TitleBarRightView", bundle: nil)
        _titleBarRightViewController.view.autoresizingMask = [.ViewHeightSizable, .ViewWidthSizable]

        _windowTitleBarView.subviews.append(_titleBarLeftViewController.view)
        _windowTitleBarView.subviews.append(_titleBarRightViewController.view)
        
        _inAppStoreWindow?.titleBarView.addSubview(_windowTitleBarView)
        _inAppStoreWindow?.titleBarHeight = 38.0
    }
    
    // NSSplitViewDelegate methods

    func splitView(splitView: NSSplitView, effectiveRect proposedEffectiveRect: NSRect, forDrawnRect drawnRect: NSRect, ofDividerAtIndex dividerIndex: Int) -> NSRect {
        return NSZeroRect
    }
    
    func splitView(splitView: NSSplitView, shouldAdjustSizeOfSubview view: NSView) -> Bool {
        if view == splitView.subviews[0] {
            return false
        }
        return true
    }
}
