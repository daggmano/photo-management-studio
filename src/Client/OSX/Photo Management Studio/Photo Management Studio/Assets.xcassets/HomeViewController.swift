//
//  HomeViewController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 2/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class HomeViewController: NSViewController, NSSplitViewDelegate {
    
    @IBOutlet weak var _mainSplitView: NSSplitView!
    @IBOutlet weak var _libraryFilterSplitView: NSSplitView!
    
    var _filterViewController: FilterViewController!
    var _libraryViewController: LibraryViewController!
    
    var _libraryFilterSplitDelegate: LibraryFilterSplitDelegate!
    
    required init?(coder: NSCoder) {
        super.init(coder: coder)
    }
    
    override init?(nibName nibNameOrNil: String?, bundle nibBundleOrNil: NSBundle?) {
        super.init(nibName: nibNameOrNil, bundle: nibBundleOrNil)
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        _libraryFilterSplitDelegate = LibraryFilterSplitDelegate()
        
        _mainSplitView.delegate = self
        
        let width = self._mainSplitView.bounds.width
        _mainSplitView.setPosition(width - 175, ofDividerAtIndex: 0)
        
        _libraryFilterSplitView.delegate = _libraryFilterSplitDelegate
        _libraryFilterSplitView.setPosition(40, ofDividerAtIndex: 0)
        
        // Set up Filter View
        _filterViewController = FilterViewController(nibName: "FilterView", bundle: nil)

        let sFilter = _libraryFilterSplitView.subviews[0].bounds.size
        _filterViewController.view.setFrameSize(NSMakeSize(sFilter.width, sFilter.height))
        _filterViewController.view.autoresizingMask = [.ViewHeightSizable, .ViewWidthSizable]
        
        _libraryFilterSplitView.subviews[0].addSubview(_filterViewController.view)
        

        // Set up Library View
        _libraryViewController = LibraryViewController(nibName: "LibraryView", bundle: nil)
        
        let sLibrary = _libraryFilterSplitView.subviews[1].bounds.size
        _libraryViewController.view.setFrameSize(NSMakeSize(sLibrary.width, sLibrary.height))
        _libraryViewController.view.autoresizingMask = [.ViewHeightSizable, .ViewWidthSizable]
        
        _libraryFilterSplitView.subviews[1].addSubview(_libraryViewController.view)
    }
    
    // NSSplitViewDelegate Methods
    func splitView(splitView: NSSplitView, constrainMinCoordinate proposedMinimumPosition: CGFloat, ofSubviewAt dividerIndex: Int) -> CGFloat {
        let width = splitView.bounds.size.width
        return width - 300
    }
    
    func splitView(splitView: NSSplitView, constrainMaxCoordinate proposedMaximumPosition: CGFloat, ofSubviewAt dividerIndex: Int) -> CGFloat {
        let width = splitView.bounds.size.width
        return width - 150
    }
    
    func splitView(splitView: NSSplitView, shouldAdjustSizeOfSubview view: NSView) -> Bool {
        if (view == splitView.subviews[1]) {
            return false
        }
        return true
    }
}
