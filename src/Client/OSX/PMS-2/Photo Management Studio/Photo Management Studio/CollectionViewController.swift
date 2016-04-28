//
//  CollectionView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 27/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class CollectionViewController: NSViewController, NSCollectionViewDelegate, NSSplitViewDelegate, NSOutlineViewDataSource, NSOutlineViewDelegate {
    
    @IBOutlet weak var collectionView: NSCollectionView!
    @IBOutlet weak var splitView: NSSplitView!
    @IBOutlet weak var outlineView: NSOutlineView!
    @IBOutlet weak var treeController: NSTreeController!
    
    static let dragType: String = "photomanagementstudio.collection.DragType"
    
    var photoItems: [PhotoItem] = []
    var viewIsCollection: Bool = false
    
    var inspectorItems: [InspectorItem] = []

    required init?(coder: NSCoder) {
        super.init(coder: coder)
    }
    
    override init?(nibName nibNameOrNil: String?, bundle nibBundleOrNil: NSBundle?) {
        super.init(nibName: nibNameOrNil, bundle: nibBundleOrNil)
    }
    
    override func awakeFromNib() {
        collectionView.registerForDraggedTypes([CollectionViewController.dragType, OutlineViewController.dragType])
        collectionView.setDraggingSourceOperationMask(.Copy, forLocal: true)
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        let nib = NSNib(nibNamed: "CollectionViewItem", bundle: nil)!
        collectionView.registerNib(nib, forItemWithIdentifier: "")
        
        Event.register("local-database-changed") { (obj) in
            self.getPhotos()
        }
        
        splitView?.subviews[1].hidden = true
        
        getPhotos()
        
        self.willChangeValueForKey("inspectorItems")
    }
    
    func getPhotos() {
        let db = Db()
        db.getAllPhotos() { response in
            switch response {
            case .Error(let error):
                print(error)
            case .Success(let response):
                self.processMedia(response.media)
            }
        }
    }
    
    func processMedia(media: [MediaObject]) {
        let serverUrl = AppDelegate.getInstance()?.getServerUrl()
        
        dispatch_async(dispatch_get_main_queue()) { 
            self.willChangeValueForKey("photoItems")
            
            self.photoItems.removeAll()
            
            for m in media {
                var thumbUrl = ""
                if let serverUrl = serverUrl {
                    print(serverUrl)
                    if let uniqueId = m.uniqueId {
                        thumbUrl = "\(serverUrl)/api/image/\(uniqueId)"
                    }
                }
                
                self.photoItems.append(PhotoItem(title: m.fileName!, subTitle: m.fullFilePath, imageUrl: thumbUrl, identifier: m.uniqueId, metadata: m.metadata))
            }
            
            self.didChangeValueForKey("photoItems")
        }
    }
    
    @IBAction func reload(sender: AnyObject?) {
        self.getPhotos()
    }
    
    @IBAction func showInfo(sender: AnyObject?) {
        if splitView.isSubviewCollapsed(splitView.subviews[1]) {
            splitView.subviews[1].hidden = false
            updateInspector()
        } else {
            splitView.subviews[1].hidden = true
        }
    }
    
    override func validateMenuItem(menuItem: NSMenuItem) -> Bool {
        let theAction = menuItem.action
        if theAction == #selector(CollectionViewController.showInfo(_:)) {
            if collectionView.selectionIndexes.count == 0 {
                return false
            }
        }
        
        return true
    }
    
    func collectionView(collectionView: NSCollectionView, didDeselectItemsAtIndexPaths indexPaths: Set<NSIndexPath>) {
        let delayTime = dispatch_time(DISPATCH_TIME_NOW, Int64(20 * Double(NSEC_PER_MSEC)))
        dispatch_after(delayTime, dispatch_get_main_queue()) { 
            self.updateInspector()
        }
    }
    
    func collectionView(collectionView: NSCollectionView, didSelectItemsAtIndexPaths indexPaths: Set<NSIndexPath>) {
        let delayTime = dispatch_time(DISPATCH_TIME_NOW, Int64(20 * Double(NSEC_PER_MSEC)))
        dispatch_after(delayTime, dispatch_get_main_queue()) {
            self.updateInspector()
        }
    }
    
    func updateInspector() {
        if (splitView.isSubviewCollapsed(splitView.subviews[1])) {
            return
        }
        
        if (collectionView.selectionIndexes.count == 0) {
            splitView.subviews[1].hidden = true
        } else {
            
            dispatch_async(dispatch_get_main_queue()) {
                self.willChangeValueForKey("inspectorItems")

                self.inspectorItems.removeAll()
            
                for idx in self.collectionView.selectionIndexes {
            
                    let photo = self.photoItems[idx]
                    let item = InspectorItem(asTitle: photo.title)
                    if let metadata = photo.meta {
                        for meta in metadata {
                            item.children.append(InspectorItem(asItem: meta.title, withValue: meta.value))
                        }
                    }
                    
                    self.inspectorItems.append(item)
                }

                self.didChangeValueForKey("inspectorItems")
                
                self.outlineView.expandItem(nil, expandChildren: true)
            }
        }
    }
    
    // MARK - Drag Operations
    
    func collectionView(collectionView: NSCollectionView, writeItemsAtIndexPaths indexPaths: Set<NSIndexPath>, toPasteboard pasteboard: NSPasteboard) -> Bool {
    
        var selectedItems: [PhotoItem] = []
        for indexPath in indexPaths {
            selectedItems.append(photoItems[indexPath.item])
        }
        
        let indexData = NSKeyedArchiver.archivedDataWithRootObject(selectedItems)
        pasteboard.clearContents()
        pasteboard.setData(indexData, forType: CollectionViewController.dragType)
        
        return true
    }
    
    // Provide small version of image being dragged to accompany mouse cursor.
    func collectionView(collectionView: NSCollectionView, draggingImageForItemsAtIndexPaths indexPaths: Set<NSIndexPath>, withEvent event: NSEvent, offset dragImageOffset: NSPointPointer) -> NSImage {
        
        let newImage = NSImage(size: NSMakeSize(CGFloat(40 * indexPaths.count), 40))
        newImage.lockFocus()
        
        var left = 0
        for indexPath in indexPaths {
            // TODO: Change source for NSImage
            let thisImage = NSImage(named: photoItems[indexPath.item].imageUrl)!.resizeImage(40, height: 40)
            
            thisImage.drawInRect(NSMakeRect(CGFloat(left), 0, 40, 40))
            left += 40
        }
        
        newImage.unlockFocus()
        
        return newImage
    }

    // MARK - Drop Operations
    
    func collectionView(collectionView: NSCollectionView, validateDrop draggingInfo: NSDraggingInfo, proposedIndexPath proposedDropIndexPath: AutoreleasingUnsafeMutablePointer<NSIndexPath?>, dropOperation proposedDropOperation: UnsafeMutablePointer<NSCollectionViewDropOperation>) -> NSDragOperation {

        let pasteboard = draggingInfo.draggingPasteboard()
        if let types = pasteboard.types {
            if (types.contains(CollectionViewController.dragType) && self.viewIsCollection) {
            
                if proposedDropOperation.memory == .On {
                    proposedDropOperation.memory = .Before
                }
            
                return NSDragOperation.Move
            
            } else if types.contains(OutlineViewController.dragType) {

                if proposedDropOperation.memory == .Before {
                    proposedDropOperation.memory = .On
                }
            
                for item in collectionView.visibleItems() {
                    item.highlightState = .None
                }
            
                if let dropTargetIndexPath = proposedDropIndexPath.memory {
                
                    let selectedDropTargets = getSelectedPathsForDropTarget(dropTargetIndexPath)
                
                    for indexPath in selectedDropTargets {
                        if collectionView.indexPathsForVisibleItems().contains(indexPath) {
                            if let target = collectionView.itemAtIndex(indexPath.item) {
                                target.highlightState = .AsDropTarget
                            }
                        }
                    }
                }
                return NSDragOperation.Copy
            }
        }
        
        return NSDragOperation.None
    }
    
    func collectionView(collectionView: NSCollectionView, acceptDrop draggingInfo: NSDraggingInfo, indexPath: NSIndexPath, dropOperation: NSCollectionViewDropOperation) -> Bool {

        for item in collectionView.visibleItems() {
            item.highlightState = .None
        }

        let pasteboard = draggingInfo.draggingPasteboard()
        let selectedDropTargets = getSelectedPathsForDropTarget(indexPath)

        guard let types = pasteboard.types else {
            return false
        }
        
        if types.contains(CollectionViewController.dragType) && self.viewIsCollection {
            // Process dropped photos
            for path in selectedDropTargets {
                let item = collectionView.itemAtIndexPath(path)
                if let obj = item?.representedObject as? PhotoItem {
                    print("Moving Target: \(obj.title)")
                }
            }
            
            print("Target Index: \(indexPath.item)")
            
            return true
        } else if types.contains(OutlineViewController.dragType) {
            
            for path in selectedDropTargets {
                let item = collectionView.itemAtIndexPath(path)
                if let obj = item?.representedObject as? PhotoItem {
                    print("Drop Target: \(obj.title)")
                }
            }
        
            if let data = pasteboard.dataForType(OutlineViewController.dragType) {
                let obj = NSKeyedUnarchiver.unarchiveObjectWithData(data) as! NSArray
                for item in obj {
                    if let o = item as? LibraryItem {
                        print("Drop Tag: \(o.text)")
                    }
                }
            }

            return true
        }
        
        return false
    }
    
    // MARK - NSSplitViewDelegate Functions
    
    func splitView(splitView: NSSplitView, effectiveRect proposedEffectiveRect: NSRect, forDrawnRect drawnRect: NSRect, ofDividerAtIndex dividerIndex: Int) -> NSRect {
        return NSZeroRect
    }
    
    // MARK - NSOutlineViewDelegate Functions
    
    func outlineView(outlineView: NSOutlineView, viewForTableColumn tableColumn: NSTableColumn?, item: AnyObject) -> NSView? {
        
        var result: NSTableCellView?
        
        if let node = item.representedObject as? InspectorItem {
            if node.isTitle() {
                let titleCellView = outlineView.makeViewWithIdentifier("TitleCell", owner: self) as? InspectorTitleCell
                titleCellView?.representedItem = node
                result = titleCellView
            } else {
                let valueCellView = outlineView.makeViewWithIdentifier("ValueCell", owner: self) as? InspectorValueCell
                valueCellView?.representedItem = node
                result = valueCellView
            }
        }
        
        return result
    }

    func outlineView(outlineView: NSOutlineView, heightOfRowByItem item: AnyObject) -> CGFloat {
        if let node = item.representedObject as? InspectorItem {
            if node.isTitle() {
                return 30.0
            }
        }
        return 42.0
    }
    
    // MARK - Private Helper Functions
    
    private func getSelectedPathsForDropTarget(indexPath: NSIndexPath) -> Set<NSIndexPath> {
        if let targetItem = collectionView.itemAtIndexPath(indexPath) {
            if targetItem.selected {
                return collectionView.selectionIndexPaths
            }
        }

        return Set<NSIndexPath>(arrayLiteral: indexPath)
    }
 }

extension NSImage {
    func resizeImage(width: CGFloat, height: CGFloat) -> NSImage {
        let img = NSImage(size: CGSizeMake(width, height))
        img.lockFocus()
        let ctx = NSGraphicsContext.currentContext()
        ctx?.imageInterpolation = .High
        drawInRect(NSRect(x: 0, y: 0, width: width, height: height), fromRect: NSRect(x: 0, y: 0, width: size.width, height: size.height), operation: .CompositeCopy, fraction: 1)
        img.unlockFocus()
        
        return img
    }
}
