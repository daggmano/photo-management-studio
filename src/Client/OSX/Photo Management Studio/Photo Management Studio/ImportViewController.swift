//
//  ImportViewController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 28/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import SwiftHTTP
import ITProgressIndicator

class ImportViewController : NSViewController, NSCollectionViewDataSource {
 
    @IBOutlet weak var collectionView: NSCollectionView!
    @IBOutlet weak var getImportablePhotosButton: NSButton!
    @IBOutlet weak var progressIndicator: ITProgressIndicator!
    @IBOutlet var sortDescriptor: NSSortDescriptor!
    
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
    
    @IBAction func getImportablePhotos(sender: AnyObject) {
//        self.importablePhotoArray.removeAllObjects()
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
    
    
    //{"itemCount":12,"importablePhotos":[{"filename":"1.dng","fullPath":"1.dng","thumbUrl":"http://10.0.0.30:5000/api/image?path=1.dng&size=200"},{"filename":"2.CR2","fullPath":"2.CR2","thumbUrl":"http://10.0.0.30:5000/api/image?path=2.CR2&size=200"},{"filename":"3.CR2","fullPath":"3.CR2","thumbUrl":"http://10.0.0.30:5000/api/image?path=3.CR2&size=200"},{"filename":"4.CR2","fullPath":"4.CR2","thumbUrl":"http://10.0.0.30:5000/api/image?path=4.CR2&size=200"},{"filename":"5.JPG","fullPath":"5.JPG","thumbUrl":"http://10.0.0.30:5000/api/image?path=5.JPG&size=200"},{"filename":"6.MOV","fullPath":"6.MOV","thumbUrl":"http://10.0.0.30:5000/api/image?path=6.MOV&size=200"},{"filename":"7.png","fullPath":"7.png","thumbUrl":"http://10.0.0.30:5000/api/image?path=7.png&size=200"},{"filename":"AndroidPhoto.jpg","fullPath":"AndroidPhoto.jpg","thumbUrl":"http://10.0.0.30:5000/api/image?path=AndroidPhoto.jpg&size=200"},{"filename":"AndroidVideo.mp4","fullPath":"AndroidVideo.mp4","thumbUrl":"http://10.0.0.30:5000/api/image?path=AndroidVideo.mp4&size=200"},{"filename":"WP_Pro.mp4","fullPath":"WP_Pro.mp4","thumbUrl":"http://10.0.0.30:5000/api/image?path=WP_Pro.mp4&size=200"},{"filename":"WP_Raw.jpg","fullPath":"WP_Raw.jpg","thumbUrl":"http://10.0.0.30:5000/api/image?path=WP_Raw.jpg&size=200"},{"filename":"WP_Raw_highres.dng","fullPath":"WP_Raw_highres.dng","thumbUrl":"http://10.0.0.30:5000/api/image?path=WP_Raw_highres.dng&size=200"}]}
    
    private func showProgressIndicator(show: Bool) {
        dispatch_async(dispatch_get_main_queue()) { () -> Void in
            self.progressIndicator.hidden = !show
            self.getImportablePhotosButton.enabled = !show
        }
    }
}
