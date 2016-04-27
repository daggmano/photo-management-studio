//
//  InspectorValueCell.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 22/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class InspectorValueCell: NSTableCellView {
    
    @IBOutlet weak var outlineView: NSOutlineView!
    @IBOutlet weak var valueField: NSTextField!
    
    var representedItem: InspectorItem?
    
    override func viewWillDraw() {
        self.textField?.stringValue = (representedItem?.title)!
        self.valueField?.stringValue = (representedItem?.value)!
    }
    
    private func getItemForRepresentedObject(object: InspectorItem) -> AnyObject? {
        let c = outlineView.numberOfRows
        
        for i in 0 ..< c {
            if let o = outlineView.itemAtRow(i)?.representedObject as? InspectorItem {
                if (o == object) {
                    return outlineView.itemAtRow(i)
                }
            }
        }
        
        return nil
    }
}
