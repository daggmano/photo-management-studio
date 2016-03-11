//
//  ImportViewController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 28/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import SwiftHTTP
import ITProgressIndicator

class ImportViewController : NSViewController, NSCollectionViewDataSource, NSCollectionViewDelegate {
 
    @IBOutlet weak var collectionView: ImportCollectionView!
    @IBOutlet weak var progressIndicator: ITProgressIndicator!
    
    @IBOutlet var sortDescriptor: NSSortDescriptor!
    var selectedIndexes: NSIndexSet!
    
    @IBOutlet weak var selectedLabel: NSTextField!
    @IBOutlet weak var totalLabel: NSTextField!
    @IBOutlet weak var importButton: NSButton!
    
    var itemSize: NSSize!
    var importablePhotoArray: NSMutableArray!
    
    required init?(coder: NSCoder) {
        super.init(coder: coder)
    }
    
    override init?(nibName nibNameOrNil: String?, bundle nibBundleOrNil: NSBundle?) {
        super.init(nibName: nibNameOrNil, bundle: nibBundleOrNil)
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        self.sortDescriptor = NSSortDescriptor(key: "filename", ascending: true, selector: "localizedCaseInsensitiveCompare:")
        self.selectedIndexes = NSIndexSet()

        self.itemSize = NSSize(width: 150, height: 180)
        self.importablePhotoArray = NSMutableArray()
        
        self.progressIndicator.isIndeterminate = true
        self.progressIndicator.lengthOfLine = 30
        self.progressIndicator.widthOfLine = 5
        self.progressIndicator.numberOfLines = 10
        self.progressIndicator.color = NSColor.lightGrayColor()
        self.progressIndicator.steppedAnimation = true
        self.progressIndicator.progress = 1
        self.progressIndicator.animationDuration = 1

        self.showProgressIndicator(false)
        
        self.collectionView.minItemSize = itemSize
        self.collectionView.maxItemSize = itemSize
    }
    
    override func viewDidAppear() {
        super.viewDidAppear()
        
        self.importButton.enabled = false
        self.getImportablePhotos()
    }
    
    func collectionView(collectionView: NSCollectionView, numberOfItemsInSection section: Int) -> Int {
        return self.importablePhotoArray.count
    }
    
    func collectionView(collectionView: NSCollectionView, itemForRepresentedObjectAtIndexPath indexPath: NSIndexPath) -> NSCollectionViewItem {
        
        let item = collectionView.makeItemWithIdentifier("PhotoViewItem", forIndexPath: indexPath)
        
        let t = self.importablePhotoArray.objectAtIndex(indexPath.item) as? ImportableItem
        print("Item is \(t?.filename) for index \(indexPath.item)")
        
        item.representedObject = self.importablePhotoArray.objectAtIndex(indexPath.item)
        
        item.view.setFrameSize(itemSize)
        
        return item
    }
    
    @IBAction func zoomIn(sender: AnyObject) {
        if (self.itemSize.width < 400) {
            self.itemSize.width += 50
            self.itemSize.height = self.itemSize.width + 30
        }
        self.collectionView.minItemSize = self.itemSize
        self.collectionView.maxItemSize = self.itemSize
        
        self.collectionView.needsLayout = true
    }
    
    @IBAction func zoomOut(sender: AnyObject) {
        if (self.itemSize.width > 100) {
            self.itemSize.width -= 50
            self.itemSize.height = self.itemSize.width + 30
        }
        self.collectionView.minItemSize = self.itemSize
        self.collectionView.maxItemSize = self.itemSize
    }
    
    @IBAction func beginImport(sender: AnyObject?) {
        
        print("Going to import the following items:")
        self.selectedIndexes.forEach { (idx) -> () in
            if let item = self.importablePhotoArray.objectAtIndex(idx) as? ImportableItem {
                print("    \(item.filename!)")
            }
        }
    }
    
    func getImportablePhotos() {

        self.showProgressIndicator(true)
        
        do {
            let app = NSApplication.sharedApplication().delegate as? AppDelegate
            if let serverUrl = app?.getServerUrl() {
                let opt = try SwiftHTTP.HTTP.GET("\(serverUrl)/api/importlist")
                opt.start { response in
                    if let err = response.error {
                        print("error: \(err.localizedDescription)")
                        self.showProgressIndicator(false)
                        return
                    }
                    
                    self.processImportListResponse(response.data)
                }
            } else {
                self.showProgressIndicator(false)
            }
        } catch let error {
            print("got an error creating the request: \(error)")
            self.showProgressIndicator(false)
        }
    }
    
    private func processImportListResponse(message: NSData) {
        do {
            if let json = try NSJSONSerialization.JSONObjectWithData(message, options: .AllowFragments) as? [String: AnyObject] {
                let jobResponse = JobResponseObject.init(json: json)
                if jobResponse.status == "submitted" {
                    if let jobId = jobResponse.jobId {
                        self.checkJobCompletion(jobId)
                    }
                }
            }
        } catch {
            self.showProgressIndicator(false)
        }
    }
    
    private func checkJobCompletion(jobId: String) {
        do {
            let app = NSApplication.sharedApplication().delegate as? AppDelegate
            if let serverUrl = app?.getServerUrl() {
                let opt = try SwiftHTTP.HTTP.GET("\(serverUrl)/api/importlist/\(jobId)")
                opt.start { response in
                    if let err = response.error {
                        print("error: \(err.localizedDescription)")
                        self.showProgressIndicator(false)
                        return
                    }
                    
                    self.processImportList(response.data)
                }
            } else {
                self.showProgressIndicator(false)
            }
        } catch let error {
            print("got an error creating the request: \(error)")
            self.showProgressIndicator(false)
        }
    }
    
    private func processImportList(message: NSData) {
        do {
            if let json = try NSJSONSerialization.JSONObjectWithData(message, options: .AllowFragments) as? [String: AnyObject] {
                let importableList = ImportableListObject.init(json: json)
                
                if let importablePhotos = importableList.importablePhotos {
                    
                    dispatch_async(dispatch_get_main_queue(), { () -> Void in
                        let array = self.mutableArrayValueForKey("importablePhotoArray")
                        array.addObjectsFromArray(importablePhotos)
                    })
                }
                
                self.showProgressIndicator(false)
            }
        } catch {
            self.showProgressIndicator(false)
        }
    }
    
    private func showProgressIndicator(show: Bool) {
        dispatch_async(dispatch_get_main_queue()) { () -> Void in
            self.progressIndicator.hidden = !show
        }
    }
    
    func collectionView(collectionView: NSCollectionView, didSelectItemsAtIndexPaths indexPaths: Set<NSIndexPath>) {
        self.updateSelectedIndexes()
    }
    
    func collectionView(collectionView: NSCollectionView, didDeselectItemsAtIndexPaths indexPaths: Set<NSIndexPath>) {
        self.updateSelectedIndexes()
    }
    
    private func updateSelectedIndexes() {
        self.selectedLabel.stringValue = "\(collectionView.selectionIndexes.count)"
        self.importButton.enabled = collectionView.selectionIndexes.count > 0
        self.selectedIndexes = collectionView.selectionIndexes
    }
}
