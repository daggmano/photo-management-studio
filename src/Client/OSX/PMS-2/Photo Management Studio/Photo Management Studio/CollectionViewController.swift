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
    
    let dragType: String = "testCollectionDragType"
    
    var photoItems: [PhotoItem] = []

    required init?(coder: NSCoder) {
        super.init(coder: coder)
    }
    
    override init?(nibName nibNameOrNil: String?, bundle nibBundleOrNil: NSBundle?) {
        super.init(nibName: nibNameOrNil, bundle: nibBundleOrNil)
    }
    
    override func awakeFromNib() {
        collectionView.registerForDraggedTypes([dragType])
        collectionView.setDraggingSourceOperationMask(.Copy, forLocal: true)
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        let nib = NSNib(nibNamed: "CollectionViewItem", bundle: nil)!
        collectionView.registerNib(nib, forItemWithIdentifier: "")
        
        self.willChangeValueForKey("photoItems")
        
        photoItems.append(PhotoItem(fileName: "FileName1", dimensions: "1920x1080", imageUrl: "Photo1"))
        photoItems.append(PhotoItem(fileName: "FileName2", dimensions: "1920x1080", imageUrl: "Photo2"))
        photoItems.append(PhotoItem(fileName: "FileName3", dimensions: "1920x1080", imageUrl: "Photo3"))
        photoItems.append(PhotoItem(fileName: "FileName4", dimensions: "1920x1080", imageUrl: "Photo4"))
        photoItems.append(PhotoItem(fileName: "FileName5", dimensions: "1920x1080", imageUrl: "Photo5"))
        photoItems.append(PhotoItem(fileName: "FileName6", dimensions: "1920x1080", imageUrl: "Photo6"))
        photoItems.append(PhotoItem(fileName: "FileName7", dimensions: "1920x1080", imageUrl: "Photo7"))
        photoItems.append(PhotoItem(fileName: "FileName8", dimensions: "1920x1080", imageUrl: "Photo8"))
        photoItems.append(PhotoItem(fileName: "FileName9", dimensions: "1920x1080", imageUrl: "Photo9"))
        photoItems.append(PhotoItem(fileName: "FileName10", dimensions: "1920x1080", imageUrl: "Photo10"))
        photoItems.append(PhotoItem(fileName: "FileName11", dimensions: "1920x1080", imageUrl: "Photo11"))
        photoItems.append(PhotoItem(fileName: "FileName12", dimensions: "1920x1080", imageUrl: "Photo12"))
        photoItems.append(PhotoItem(fileName: "FileName13", dimensions: "1920x1080", imageUrl: "Photo13"))
        
        self.didChangeValueForKey("photoItems")
    }
    
    
    func collectionView(collectionView: NSCollectionView, writeItemsAtIndexPaths indexPaths: Set<NSIndexPath>, toPasteboard pasteboard: NSPasteboard) -> Bool {
        
        var selectedItems: [PhotoItem] = []
        for indexPath in indexPaths {
            selectedItems.append(photoItems[indexPath.item])
        }
        
        let indexData = NSKeyedArchiver.archivedDataWithRootObject(selectedItems)
  
        pasteboard.setData(indexData, forType: "testTreeDragType")
//        NSPasteboard.generalPasteboard().clearContents()
//        NSPasteboard.generalPasteboard().declareTypes([kUTTypeText as String, kUTTypeData as String], owner: nil)
//        NSPasteboard.generalPasteboard().setString("photos", forType: (kUTTypeText as String))
//        NSPasteboard.generalPasteboard().setData(indexData, forType: (kUTTypeData as String))
        
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
    
    // Image is dropped on destination NSCollectionView.
//    func collectionView(collectionView: NSCollectionView, draggingSession session: NSDraggingSession, endedAtPoint screenPoint: NSPoint, dragOperation operation: NSDragOperation) {

//        for item in collectionView.visibleItems() {
//            item.highlightState = .None
//        }

        
//        let pasteboardItem = NSPasteboard.generalPasteboard().pasteboardItems![0]
//        let urlString = pasteboardItem.stringForType((kUTTypeText as String))
        //let imageData = pasteboardItem.dataForType((kUTTypeData as String))
        
        // destinationImages is the data source for the destination collectionView. destinationImageURLs is used to keep track of the text urls.
//        if urlString != nil {
//            print(urlString)
//            destinationImageURLs.insert(urlString!, atIndex: 0)
//            destinationImages.insert(NSImage(data: imageData!)!, atIndex: 0)
//            destinationCollectionView.reloadData()
//            let selectionRect = self.favoritesCollectionView.frameForItemAtIndex(0)
//            destinationCollectionView.scrollRectToVisible(selectionRect)
//        }
//    }
    
//    func collectionView(collectionView: NSCollectionView, canDragItemsAtIndexes indexes: NSIndexSet, withEvent event: NSEvent) -> Bool {
//        print("Can Drag indexes : \(indexes)")
//        return true
//    }
    
    
    
//    func collectionView(collectionView: NSCollectionView, acceptDrop draggingInfo: NSDraggingInfo, index: Int, dropOperation: NSCollectionViewDropOperation) -> Bool {

//        for item in collectionView.visibleItems() {
//            item.highlightState = .None
//        }

//        print("Accept Drop : \(index)")
        
//        NSPasteboard *pBoard = [draggingInfo draggingPasteboard];
//        NSData *indexData = [pBoard dataForType:@"my_drag_type_id"];
//        NSIndexSet *indexes = [NSKeyedUnarchiver unarchiveObjectWithData:indexData];
//        NSInteger draggedCell = [indexes firstIndex];
        // Now we know the Original Index (draggedCell) and the
        // index of destination (index). Simply swap them in the collection view array.

//        return true
//    }
    
//    func collectionView(collectionView: NSCollectionView, acceptDrop draggingInfo: NSDraggingInfo, indexPath: NSIndexPath, dropOperation: NSCollectionViewDropOperation) -> Bool {
//        for item in collectionView.visibleItems() {
//            item.highlightState = .None
//        }
        
//        print(indexPath)
//        print(dropOperation)
        
//        return true
//    }
    
//    func collectionView(collectionView: NSCollectionView, acceptDrop draggingInfo: NSDraggingInfo, indexPath: NSIndexPath, dropOperation: NSCollectionViewDropOperation) -> Bool {
//        print("a")
//        return true
//    }
    
//    func collectionView(collectionView: NSCollectionView, acceptDrop draggingInfo: NSDraggingInfo, index: Int, dropOperation: NSCollectionViewDropOperation) -> Bool {
//        print("b")
//        return true
//    }
    
    
    func collectionView(collectionView: NSCollectionView, validateDrop draggingInfo: NSDraggingInfo, proposedIndexPath proposedDropIndexPath: AutoreleasingUnsafeMutablePointer<NSIndexPath?>, dropOperation proposedDropOperation: UnsafeMutablePointer<NSCollectionViewDropOperation>) -> NSDragOperation {
        if proposedDropOperation.memory == .Before {
            proposedDropOperation.memory = .On
        }

        for item in collectionView.visibleItems() {
            item.highlightState = .None
        }
        
        if let dropTargetIndexPath = proposedDropIndexPath.memory {
            if collectionView.indexPathsForVisibleItems().contains(dropTargetIndexPath) {
                if let target = collectionView.itemAtIndex(dropTargetIndexPath.item) {
                    target.highlightState = .AsDropTarget
                }
            }
        }

        return NSDragOperation.Copy
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
