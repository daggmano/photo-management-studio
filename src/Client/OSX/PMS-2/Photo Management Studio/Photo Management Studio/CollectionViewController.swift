//
//  CollectionView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 27/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class CollectionViewController: NSViewController, NSCollectionViewDelegate {
    
    @IBOutlet weak var collectionView: NSCollectionView!
    
    static let dragType: String = "photomanagementstudio.collection.DragType"
    
    var photoItems: [PhotoItem] = []
    var viewIsCollection: Bool = false

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
        
        self.willChangeValueForKey("photoItems")
        
        photoItems.append(PhotoItem(title: "FileName1", subTitle: "1920x1080", imageUrl: "Photo1", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName2", subTitle: "1920x1080", imageUrl: "Photo2", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName3", subTitle: "1920x1080", imageUrl: "Photo3", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName4", subTitle: "1920x1080", imageUrl: "Photo4", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName5", subTitle: "1920x1080", imageUrl: "Photo5", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName6", subTitle: "1920x1080", imageUrl: "Photo6", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName7", subTitle: "1920x1080", imageUrl: "Photo7", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName8", subTitle: "1920x1080", imageUrl: "Photo8", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName9", subTitle: "1920x1080", imageUrl: "Photo9", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName10", subTitle: "1920x1080", imageUrl: "Photo10", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName11", subTitle: "1920x1080", imageUrl: "Photo11", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName12", subTitle: "1920x1080", imageUrl: "Photo12", identifier: nil))
        photoItems.append(PhotoItem(title: "FileName13", subTitle: "1920x1080", imageUrl: "Photo13", identifier: nil))
        
        self.didChangeValueForKey("photoItems")
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
