//
//  InspectorTitleCell.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 22/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class InspectorTitleCell: NSTableCellView {
    
    @IBOutlet weak var outlineView: NSOutlineView!
    
    var representedItem: InspectorItem?
    
    override func viewWillDraw() {
        if let title = representedItem?.title {
            self.textField?.stringValue = title.uppercaseString
        }
    }
}
