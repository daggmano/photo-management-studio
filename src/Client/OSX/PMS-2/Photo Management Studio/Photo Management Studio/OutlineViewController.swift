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
    
    static let dragType: String = "photomanagementstudio.taglist.DragType"
    
    var libraryItems: [LibraryItem] = []
    
    override func awakeFromNib() {
        outlineView.registerForDraggedTypes([CollectionViewController.dragType])
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do view setup here.
        
        self.willChangeValueForKey("libraryItems")
        
        libraryItems.append(LibraryItem(asTitle: "Tags", canAdd: true))
        libraryItems.append(LibraryItem(asTitle: "Collections", canAdd: true))
        libraryItems.append(LibraryItem(asTitle: "Imports", canAdd: false))
        
        self.didChangeValueForKey("libraryItems")
        
        Event.register("local-database-changed") { (obj) in
            self.refreshImports()
        }
        
        refreshImports()
    }
    
    func refreshImports() {
        let db = Db()
        db.getAllImports() { response in
            switch response {
            case .Error(let error):
                print(error)
            case .Success(let response):
                self.processImports(response.imports)
            }
        }
    }
    
    func processImports(imports: [ImportObject]) {
    
        print("Importing \(imports.count) imports")
        
        dispatch_async(dispatch_get_main_queue()) { 
            self.willChangeValueForKey("libraryItems")
            
            let dateFormatter = NSDateFormatter()
            dateFormatter.dateFormat = "EEE, d MMM yyyy h:mm a"
            
            let importParent = self.libraryItems[2]
            importParent.children.removeAll()
            
            for imp in imports {
                if let importDate = imp.importDate {
                    print(dateFormatter.stringFromDate(importDate))
                    importParent.children.append(LibraryItem(asItem: dateFormatter.stringFromDate(importDate), canTag: false))
                }
            }
            
            self.didChangeValueForKey("libraryItems")
        }
    }
    
    func outlineView(outlineView: NSOutlineView, viewForTableColumn tableColumn: NSTableColumn?, item: AnyObject) -> NSView? {

        var result: NSTableCellView?

        if let node = item.representedObject as? LibraryItem {
            if node.isTitle() {
                let titleCellView = outlineView.makeViewWithIdentifier("TitleCell", owner: self) as? TitleCellView
                titleCellView?.representedItem = node
                result = titleCellView
            } else {
                result = outlineView.makeViewWithIdentifier("ItemCell", owner: self) as? NSTableCellView
                result?.textField?.stringValue = node.text

                if node.canTag {
                    result?.imageView?.image = NSImage(named: "Photo1")
                }
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
    
    // MARK - Drag Operations
    
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
            pasteboard.clearContents()
            pasteboard.setData(data, forType: OutlineViewController.dragType)
        }
        
        return itemsAreDraggable
    }
    
    // MARK - Drop Operations
    
    func outlineView(outlineView: NSOutlineView, validateDrop info: NSDraggingInfo, proposedItem item: AnyObject?, proposedChildIndex index: Int) -> NSDragOperation {

        if index != -1 || item == nil {
            return NSDragOperation.None
        }
        
        guard let node = item?.representedObject as? LibraryItem else {
            return NSDragOperation.None
        }

        let pasteboard = info.draggingPasteboard()
        if let types = pasteboard.types {
            if types.contains(CollectionViewController.dragType) && node.canTag {
                return NSDragOperation.Copy
            }
        }

        return NSDragOperation.None
    }
    
    func outlineView(outlineView: NSOutlineView, acceptDrop info: NSDraggingInfo, item: AnyObject?, childIndex index: Int) -> Bool {

        let pasteboard = info.draggingPasteboard()
        
        guard let types = pasteboard.types else {
            return false
        }
        
        if types.contains(CollectionViewController.dragType) {
            
            if let data = pasteboard.dataForType(CollectionViewController.dragType) {
                let obj = NSKeyedUnarchiver.unarchiveObjectWithData(data) as! NSArray
                for item in obj {
                    if let o = item as? PhotoItem {
                        print("Dropped Photo: \(o.title)")
                    }
                }
            }

            if let item = item?.representedObject as? LibraryItem {
                print("Target Item: \(item.text)")
            }
            
            return true
        }
        
        return false
    }
}
