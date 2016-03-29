//
//  OutlineViewController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 20/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class OutlineViewController: NSViewController, NSOutlineViewDelegate, NSOutlineViewDataSource {

    @IBOutlet weak var outlineView: NSOutlineView!
    @IBOutlet weak var treeController: NSTreeController!
    
    let dragType: String = "testTreeDragType"
    
    var libraryItems: [LibraryItem] = []
    
    override func awakeFromNib() {
        outlineView.registerForDraggedTypes([dragType])
    }
    
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
    
    // MARK: - NSOutlineView Data Source Helper Functions
    
//    func childrenOfNode(node : AnyObject?) -> [LibraryItem]? {
        // 1. If we have a node then return it's children
        // 2. Else we need to locate the root node and try to return it's children
        // 3. Finally we exhaust our choices and return nil
//        if node != nil {
//            let item: LibraryItem! = node as! LibraryItem
//            let children: [LibraryItem] = item.children
//            return children
//        } else if let rootTreeNode : NSTreeNode = treeController.arrangedObjects.descendantNodeAtIndexPath(NSIndexPath(index: 0)) {
//            if let rootNode: LibraryItem = rootTreeNode.representedObject as? LibraryItem {
//                return rootNode.children
//            }
//        }

//        return nil
//    }
    
//    func rootNode() -> LibraryItem? {
//        if let rootTreeNode : NSTreeNode = treeController.arrangedObjects.descendantNodeAtIndexPath(NSIndexPath(index: 0)) {
//            return rootTreeNode.representedObject as? LibraryItem
//        } else {
//            return nil
//        }
//    }
    
//    func isLeaf(item : AnyObject?) -> Bool {
//        if item != nil {
//            if let children : [LibraryItem] = item?.children {
//                if children.count == 0 {
//                    return true
//                } else {
//                    return false
//                }
//            }
//        }
        // item is nil
//        return false
//    }
    
    // MARK: - NSOutlineView Drag and Drop Required Functions
    
    func outlineView(outlineView: NSOutlineView, writeItems items: [AnyObject], toPasteboard pasteboard: NSPasteboard) -> Bool {
        
        //get an array of URIs for the selected objects
        var itemsAreDraggable = true
        
        print(items)
        let mutableArray = NSMutableArray()
        
        for object in items {
            if let treeItem = object.representedObject as? LibraryItem {
                if treeItem.children.count > 0 {
                    itemsAreDraggable = false
                } else {
                    mutableArray.addObject(treeItem)
                }
            }
        }

        if itemsAreDraggable {
            let data = NSKeyedArchiver.archivedDataWithRootObject(mutableArray)
            pasteboard.setData(data, forType: "testCollectionDragType")
        }
        
        return itemsAreDraggable
    }
    
    func outlineView(outlineView: NSOutlineView, validateDrop info: NSDraggingInfo, proposedItem item: AnyObject?, proposedChildIndex index: Int) -> NSDragOperation {
        print(info)
        print(item)
        return NSDragOperation.Move
    }
    
    func outlineView(outlineView: NSOutlineView, acceptDrop info: NSDraggingInfo, item: AnyObject?, childIndex index: Int) -> Bool {
        
        // Determine the parent node
//        var parentItem : AnyObject? = item?.representedObject
        
        // Get Dragged NSTreeNodes
//        let pasteboard : NSPasteboard = info.draggingPasteboard()
//        let data : NSData = pasteboard.dataForType(dragType)! as NSData
//        let draggedArray : NSArray? = NSKeyedUnarchiver.unarchiveObjectWithData(data) as? NSArray
        
        // Loop through DraggedArray
//        for object : AnyObject in draggedArray! {
            // Get the ID of the NSManagedObject
//            if let id : NSManagedObjectID? = persistentStoreCoordinator?.managedObjectIDForURIRepresentation(object as NSURL){
                // Set new parent to the dragged item
//                if let treeItem = managedObjectContext?.objectWithID(id!){
//                    treeItem.setValue(parentItem, forKey: "parent")
//                }
//            }
//        }
        
        // Reload the OutlineView
//        outlineView.reloadData()
        
        return true
    }
}
