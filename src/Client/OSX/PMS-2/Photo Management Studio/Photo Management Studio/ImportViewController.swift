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
    
    var progressIndicatorView: ProgressIndicator!

    var photoItems: [PhotoItem] = []
    var importablePhotos: NSArray!
    var selectedIndexes: NSIndexSet!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        let nib = NSNib(nibNamed: "ImportViewItem", bundle: nil)!
        collectionView.registerNib(nib, forItemWithIdentifier: "")
        selectedIndexes = NSIndexSet()
        
        progressIndicatorView = ProgressIndicator(progressIndicator: progressIndicator)
        
        progressIndicatorView.show(false)
        
        self.updateSelectedIndexes()
    }
    
    override func viewDidAppear() {
        getImportablePhotos()
    }
    
    func getImportablePhotos() {
        
        progressIndicatorView.show(true)
        
        if let serverUrl = AppDelegate.getInstance()?.getServerUrl() {
            Alamofire.request(.GET, "\(serverUrl)/api/importlist").responseJSON { response in
                
                var statusCode = 0
                if let response: NSHTTPURLResponse = response.response! as NSHTTPURLResponse {
                    statusCode = response.statusCode
                }
                
                if (statusCode == HTTPStatusCode.OK.rawValue) {
                    self.processImportListResponse(response.data!)
                } else {
                    self.progressIndicatorView.show(false)
                }
            }
        } else {
            progressIndicatorView.show(false)
        }
    }
    
    func selectNone(sender: AnyObject?) {
        collectionView.selectionIndexPaths.removeAll()
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
            progressIndicatorView.show(false)
        }
    }
    
    private func checkImportListJobCompletion(jobId: String) {
        
        if let serverUrl = AppDelegate.getInstance()?.getServerUrl() {
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
                    self.progressIndicatorView.show(false)
                }
            }
        } else {
            progressIndicatorView.show(false)
        }
    }
    
    private func processImportList(message: NSData) {
        do {
            if let json = try NSJSONSerialization.JSONObjectWithData(message, options: .AllowFragments) as? [String: AnyObject] {
                let importableList = ImportableListObject.init(json: json)
                
                if let importablePhotos = importableList.importablePhotos {
                    self.importablePhotos = importablePhotos
                    
                    dispatch_async(dispatch_get_main_queue(), { () -> Void in
                        self.willChangeValueForKey("photoItems")
                    
                        self.photoItems.removeAll()
                        for item in importablePhotos {
                            self.photoItems.append(PhotoItem(title: item.filename!, subTitle: item.fullPath!, imageUrl: item.thumbUrl!, identifier: item.fullPath!, metadata: nil))
                        }
                    
                        self.didChangeValueForKey("photoItems")
                        print(self.photoItems)
                    })
                }
                
                progressIndicatorView.show(false)
            }
        } catch {
            progressIndicatorView.show(false)
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
    
    @IBAction func closeSheet(sender: AnyObject?) {
        self.view.window?.sheetParent?.endSheet(self.view.window!, returnCode: NSModalResponseOK)
    }
    
    @IBAction func performPhotoImport(sender: AnyObject) {
        
        var photoPaths: [String] = [];
        self.selectedIndexes.forEach { (idx) -> () in
            if let item = self.importablePhotos.objectAtIndex(idx) as? ImportableItem {
                if let fullPath = item.fullPath {
                    photoPaths.append(fullPath)
                }
            }
        }
        
        let request = ImportPhotosRequestObject(photoPaths: photoPaths)
        
        progressIndicatorView.show(true)
        
        if let serverUrl = AppDelegate.getInstance()?.getServerUrl() {
            Alamofire.request(.POST, "\(serverUrl)/api/import", parameters: request.toJSON(), encoding: .JSON).responseJSON { response in
                
                var statusCode = 0
                if let response: NSHTTPURLResponse = response.response! as NSHTTPURLResponse {
                    statusCode = response.statusCode
                }
                
                if statusCode == HTTPStatusCode.OK.rawValue {
                    self.processImportResponse(response.data!)
                } else {
                    self.progressIndicatorView.show(false)
                }
            }
        } else {
            progressIndicatorView.show(false)
        }
    }
    
    private func processImportResponse(message: NSData) {
        do {
            if let json = try NSJSONSerialization.JSONObjectWithData(message, options: .AllowFragments) as? [String: AnyObject] {
                let jobResponse = JobResponseObject.init(json: json)
                if jobResponse.status == "submitted" {
                    if let jobId = jobResponse.jobId {
                        self.checkImportJobCompletion(jobId)
                    }
                }
            }
        } catch {
            progressIndicatorView.show(false)
        }
    }
    
    private func checkImportJobCompletion(jobId: String) {
        
        if let serverUrl = AppDelegate.getInstance()?.getServerUrl() {
            
            Alamofire.request(.GET, "\(serverUrl)/api/import/\(jobId)").responseJSON { response in
                
                var statusCode = 0
                if let response: NSHTTPURLResponse = response.response! as NSHTTPURLResponse {
                    statusCode = response.statusCode
                }
                
                if statusCode == HTTPStatusCode.PreconditionFailed.rawValue {
                    let delay_time = dispatch_time(DISPATCH_TIME_NOW, Int64(1 * Double(NSEC_PER_SEC)))
                    dispatch_after(delay_time, dispatch_get_main_queue(), { () -> Void in
                        self.checkImportJobCompletion(jobId)
                    })
                } else if statusCode == HTTPStatusCode.OK.rawValue {
                    self.processImport(response.data!)
                } else {
                    self.progressIndicatorView.show(false)
                }
            }
        } else {
            progressIndicatorView.show(false)
        }
    }
    
    private func processImport(message: NSData) {
        do {
            if let json = try NSJSONSerialization.JSONObjectWithData(message, options: .AllowFragments) as? [String: AnyObject] {
                let importedList = ImportedFilesObject.init(json: json)
                
                if let importedPhotos = importedList.importedPhotos {
                    
                    print(importedPhotos)
                    
                    dispatch_async(dispatch_get_main_queue(), { () -> Void in
                        
                        self.getImportablePhotos()
                    })
                }
                
                progressIndicatorView.show(false)
            }
        } catch {
            progressIndicatorView.show(false)
        }
    }
}
