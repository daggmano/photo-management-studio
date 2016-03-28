//
//  OutlineViewController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 20/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class OutlineViewController: NSViewController, NSOutlineViewDelegate {

    @IBOutlet weak var outlineView: NSOutlineView!
    @IBOutlet weak var treeController: NSTreeController!
    
    var libraryItems: [LibraryItem] = []
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do view setup here.
        
        self.willChangeValueForKey("libraryItems")
        
        let item = LibraryItem(asTitle: "Tags")
        item.children.append(LibraryItem(asItem: "Item 1"))
        item.children.append(LibraryItem(asItem: "Item 2"))
        
        item.children[1].children.append(LibraryItem(asItem: "Sub Item 2.1"))
        item.children[1].children[0].children.append(LibraryItem(asItem: "Sub Item 2.1.1"))
        item.children[1].children[0].children[0].children.append(LibraryItem(asItem: "Sub Item 2.1.1.1"))
        item.children[1].children[0].children[0].children[0].children.append(LibraryItem(asItem: "Sub Item 2.1.1.1.1"))
        
        libraryItems.append(item)
        libraryItems.append(LibraryItem(asTitle: "Collections"))
        libraryItems.append(LibraryItem(asTitle: "Imports"))
        
        self.didChangeValueForKey("libraryItems")
    }
    
    func outlineView(outlineView: NSOutlineView, viewForTableColumn tableColumn: NSTableColumn?, item: AnyObject) -> NSView? {

        var result: NSTableCellView?

        if let node = item.representedObject as? LibraryItem {
            if node.isTitle() {
                let titleCellView = outlineView.makeViewWithIdentifier("TitleCell", owner: self) as? TitleCellView
                titleCellView?.representedItem = node
                result = titleCellView
            } else if node.isSeparator() {
                result = outlineView.makeViewWithIdentifier("SeparatorCell", owner: self) as? NSTableCellView
            } else {
                result = outlineView.makeViewWithIdentifier("ItemCell", owner: self) as? NSTableCellView
                let value = node.text!
                result?.textField?.stringValue = value
            }
        }
        
        return result
    }
    
    func outlineView(outlineView: NSOutlineView, shouldSelectItem item: AnyObject) -> Bool {
        
        if let node = item.representedObject as? LibraryItem {
            return !node.isTitle()
        }
        
        return false
    }
}
