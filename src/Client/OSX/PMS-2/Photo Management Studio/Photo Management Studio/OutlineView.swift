//
//  OutlineView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 23/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class OutlineView: NSOutlineView {

    override func frameOfOutlineCellAtRow(row: Int) -> NSRect {
        return NSZeroRect
    }
    
    override func frameOfCellAtColumn(column: Int, row: Int) -> NSRect {
        let superFrame = super.frameOfCellAtColumn(column, row: row)
        

        if let item = self.itemAtRow(row)?.representedObject as? LibraryItem {
            if item.isTitle() {
                return NSMakeRect(0, NSMinY(superFrame), NSWidth(superFrame) + NSMinX(superFrame), NSHeight(superFrame))
            } else {
                return NSMakeRect(NSMinX(superFrame) - 8, NSMinY(superFrame), NSWidth(superFrame) + 8, NSHeight(superFrame))
            }
        }
        
        return superFrame
    }
}
