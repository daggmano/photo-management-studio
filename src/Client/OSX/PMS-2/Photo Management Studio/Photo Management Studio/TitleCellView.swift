//
//  TitleCell.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 23/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class TitleCellView: NSTableCellView {
    
    @IBOutlet weak var outlineView: NSOutlineView!
    @IBOutlet weak var titleButton: NSButton!
    @IBOutlet weak var addButton: NSButton!
    
    var representedItem: LibraryItem?
    
    override func viewWillDraw() {
        self.titleButton.title = (representedItem?.text)!.uppercaseString

        addButton.hidden = !representedItem!.titleHasAdd()
    }
    
    @IBAction func onShowHide(sender: AnyObject?) {
        if let button = sender as? NSButton {
            if let item = getItemForRepresentedObject(representedItem!) {
            
                if (button.state == 1) {
                    outlineView.expandItem(item, expandChildren: true)
                } else {
                    outlineView.collapseItem(item, collapseChildren: true)
                }
            }
        }
    }
    
    private func getItemForRepresentedObject(object: LibraryItem) -> AnyObject? {
        let c = outlineView.numberOfRows
        
        for i in 0 ..< c {
            if let o = outlineView.itemAtRow(i)?.representedObject as? LibraryItem {
                if (o == object) {
                    return outlineView.itemAtRow(i)
                }
            }
        }
        
        return nil
    }
}
