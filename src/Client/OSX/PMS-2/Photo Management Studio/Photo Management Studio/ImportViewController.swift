//
//  ImportViewController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 3/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Alamofire
import Cocoa
import ITProgressIndicator

class ImportViewController: NSViewController, NSCollectionViewDelegate {

    @IBOutlet weak var collectionView: NSCollectionView!
    @IBOutlet weak var importButton: NSButton!
    @IBOutlet weak var progressIndicator: ITProgressIndicator!

    var photoItems: [PhotoItem] = []
    var selectedIndexes: NSIndexSet!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        let nib = NSNib(nibNamed: "CollectionViewItem", bundle: nil)!
        collectionView.registerNib(nib, forItemWithIdentifier: "")
        selectedIndexes = NSIndexSet()
        
        self.progressIndicator.isIndeterminate = true
        self.progressIndicator.lengthOfLine = 30
        self.progressIndicator.widthOfLine = 5
        self.progressIndicator.numberOfLines = 10
        self.progressIndicator.color = NSColor.lightGrayColor()
        self.progressIndicator.steppedAnimation = true
        self.progressIndicator.progress = 1
        self.progressIndicator.animationDuration = 1
        
        self.showProgressIndicator(false)
        
//        self.willChangeValueForKey("photoItems")
        
//        photoItems.append(PhotoItem(fileName: "FileName1", dimensions: "1920x1080", imageUrl: "Photo1"))
//        photoItems.append(PhotoItem(fileName: "FileName2", dimensions: "1920x1080", imageUrl: "Photo2"))
//        photoItems.append(PhotoItem(fileName: "FileName3", dimensions: "1920x1080", imageUrl: "Photo3"))
//        photoItems.append(PhotoItem(fileName: "FileName4", dimensions: "1920x1080", imageUrl: "Photo4"))
//        photoItems.append(PhotoItem(fileName: "FileName5", dimensions: "1920x1080", imageUrl: "Photo5"))
//        photoItems.append(PhotoItem(fileName: "FileName6", dimensions: "1920x1080", imageUrl: "Photo6"))
//        photoItems.append(PhotoItem(fileName: "FileName7", dimensions: "1920x1080", imageUrl: "Photo7"))
//        photoItems.append(PhotoItem(fileName: "FileName8", dimensions: "1920x1080", imageUrl: "Photo8"))
//        photoItems.append(PhotoItem(fileName: "FileName9", dimensions: "1920x1080", imageUrl: "Photo9"))
//        photoItems.append(PhotoItem(fileName: "FileName10", dimensions: "1920x1080", imageUrl: "Photo10"))
//        photoItems.append(PhotoItem(fileName: "FileName11", dimensions: "1920x1080", imageUrl: "Photo11"))
//        photoItems.append(PhotoItem(fileName: "FileName12", dimensions: "1920x1080", imageUrl: "Photo12"))
//        photoItems.append(PhotoItem(fileName: "FileName13", dimensions: "1920x1080", imageUrl: "Photo13"))
        
//        self.didChangeValueForKey("photoItems")
        
        self.updateSelectedIndexes()
    }
    
    override func viewDidAppear() {
        getImportablePhotos()
    }
    
    func getImportablePhotos() {
        
        self.showProgressIndicator(true)
        
        if let serverUrl = AppDelegate.getInstance()?.getServerUrl() {
            Alamofire.request(.GET, "\(serverUrl)/api/importlist").responseJSON { response in
                
                var statusCode = 0
                if let response: NSHTTPURLResponse = response.response! as NSHTTPURLResponse {
                    statusCode = response.statusCode
                }
                
                if (statusCode == HTTPStatusCode.OK.rawValue) {
                    self.processImportListResponse(response.data!)
                } else {
                    self.showProgressIndicator(false)
                }
            }
        } else {
            self.showProgressIndicator(false)
        }
    }
    
    private func processImportListResponse(message: NSData) {
        do {
            if let json = try NSJSONSerialization.JSONObjectWithData(message, options: .AllowFragments) as? [String: AnyObject] {
                let jobResponse = JobResponseObject.init(json: json)
                if jobResponse.status == "submitted" {
                    if let jobId = jobResponse.jobId {
                        self.checkImportListJobCompletion(jobId)
                    }
                }
            }
        } catch {
            self.showProgressIndicator(false)
        }
    }
    
    private func checkImportListJobCompletion(jobId: String) {
        
        let app = NSApplication.sharedApplication().delegate as? AppDelegate
        if let serverUrl = app?.getServerUrl() {
            Alamofire.request(.GET, "\(serverUrl)/api/importlist/\(jobId)").responseJSON { response in
                
                var statusCode = 0
                if let response: NSHTTPURLResponse = response.response! as NSHTTPURLResponse {
                    statusCode = response.statusCode
                }
                
                if (statusCode == HTTPStatusCode.PreconditionFailed.rawValue) {
                    let delay_time = dispatch_time(DISPATCH_TIME_NOW, Int64(1 * Double(NSEC_PER_SEC)))
                    dispatch_after(delay_time, dispatch_get_main_queue(), { () -> Void in
                        self.checkImportListJobCompletion(jobId)
                    })
                } else if (statusCode == HTTPStatusCode.OK.rawValue) {
                    self.processImportList(response.data!)
                } else {
                    self.showProgressIndicator(false)
                }
            }
        } else {
            self.showProgressIndicator(false)
        }
    }
    
    private func processImportList(message: NSData) {
        do {
            if let json = try NSJSONSerialization.JSONObjectWithData(message, options: .AllowFragments) as? [String: AnyObject] {
                let importableList = ImportableListObject.init(json: json)
                
                if let importablePhotos = importableList.importablePhotos {
                    
                    dispatch_async(dispatch_get_main_queue(), { () -> Void in
                        self.willChangeValueForKey("photoItems")
                    
                        self.photoItems.removeAll()
                        for item in importablePhotos {
                            self.photoItems.append(PhotoItem(title: item.filename!, subTitle: item.fullPath!, imageUrl: item.thumbUrl!, identifier: nil))
                        }
                    
                        self.didChangeValueForKey("photoItems")
                        print(self.photoItems)
                    })
//                    dispatch_async(dispatch_get_main_queue(), { () -> Void in
//                        let array = self.mutableArrayValueForKey("importablePhotoArray")
//                        array.addObjectsFromArray(importablePhotos)
//                    })
                }
                
                self.showProgressIndicator(false)
            }
        } catch {
            self.showProgressIndicator(false)
        }
    }

    func collectionView(collectionView: NSCollectionView, didSelectItemsAtIndexPaths indexPaths: Set<NSIndexPath>) {
        self.updateSelectedIndexes()
    }
    
    func collectionView(collectionView: NSCollectionView, didDeselectItemsAtIndexPaths indexPaths: Set<NSIndexPath>) {
        self.updateSelectedIndexes()
    }
    
    private func updateSelectedIndexes() {
        dispatch_async(dispatch_get_main_queue()) { 
            self.importButton.title = "Import \(self.collectionView.selectionIndexes.count) photos"
            self.importButton.enabled = self.collectionView.selectionIndexes.count > 0
        }
        self.selectedIndexes = collectionView.selectionIndexes
    }
    
    private func showProgressIndicator(show: Bool) {
        dispatch_async(dispatch_get_main_queue()) { () -> Void in
            self.progressIndicator.hidden = !show
        }
    }
    
    @IBAction func closeSheet(sender: AnyObject?) {
        self.view.window?.sheetParent?.endSheet(self.view.window!, returnCode: NSModalResponseOK)
    }
    
    @IBAction func performPhotoImport(sender: AnyObject) {
        
    }
}
