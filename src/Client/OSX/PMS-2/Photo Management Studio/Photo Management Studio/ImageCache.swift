//
//  ImageCache.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 28/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class ImageCache: NSObject {

    class func getImage(url: String, thumbName: String, useCache: Bool, callback: ((NSImage) -> Void)) {
        
        print("getImage: url: \(url), thumbName: \(thumbName)")
        
        dispatch_async(dispatch_queue_create("getAsyncPhotosGCDQueue", nil), { () -> Void in
            if !useCache {
                if let url = NSURL(string: url) {
                    if let image = NSImage(contentsOfURL: url) {
                        dispatch_async(dispatch_get_main_queue(), { () -> Void in
                            callback(image)
                        })
                    }
                }
            } else {
                if let thumbPath = self.getCachePath(thumbName) {
                    print("thumbPath: \(thumbPath)")
                    if NSFileManager.defaultManager().fileExistsAtPath(thumbPath) {
                        if let image = NSImage(contentsOfFile: thumbPath) {
                            dispatch_async(dispatch_get_main_queue(), { () -> Void in
                                callback(image)
                            })
                        } else {
                            do {
                                try NSFileManager.defaultManager().removeItemAtPath(thumbPath)
                                self.getImage(url, thumbName: thumbName, useCache: useCache, callback: callback)
                            } catch {}
                        }
                    } else {
                        if let url = NSURL(string: url) {
                            if let image = NSImage(contentsOfURL: url) {
                                image.saveAsJpegWithPath(thumbPath)
                                dispatch_async(dispatch_get_main_queue(), { () -> Void in
                                    callback(image)
                                })
                            }
                        }
                    }
                }
            }
        })
        
    }
    
    private class func getCachePath(fileName: String) -> String? {
        
        let documentsPath = NSSearchPathForDirectoriesInDomains(.DocumentDirectory, .UserDomainMask, true)[0]
        print("documentsPath: \(documentsPath)")
        let tempDirURL =  NSURL(string: documentsPath)!.URLByAppendingPathComponent(Config.cacheFolder)
        print("tempDirURL: \(tempDirURL.absoluteString)")
        do {
            try NSFileManager.defaultManager().createDirectoryAtPath(tempDirURL.absoluteString, withIntermediateDirectories: true, attributes: nil)
        } catch {
            return nil
        }
        
        return tempDirURL.URLByAppendingPathComponent(fileName).absoluteString
    }
}
