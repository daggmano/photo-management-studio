//
//  ImportableListObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class ImportableListObject : NSObject, JsonProtocol {
    internal private(set) var itemCount: Int?
    internal private(set) var importablePhotos: [ImportableItem]?
    
    init(itemCount: Int, importablePhotos: [ImportableItem]) {
        self.itemCount = itemCount
        self.importablePhotos = importablePhotos
    }
    
    required init(json: [String: AnyObject]) {
        self.itemCount = json["itemCount"] as? Int
        if let importablePhotos = json["importablePhotos"] as? [[String: AnyObject]] {
            self.importablePhotos = [ImportableItem]()
            
            for item in importablePhotos {
                self.importablePhotos!.append(ImportableItem(json: item))
            }
         }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let itemCount = self.itemCount {
            result["itemCount"] = itemCount
        }
        if let importablePhotos = self.importablePhotos {
            var array = [[String: AnyObject]]()
            for item in importablePhotos {
                array.append(item.toJSON())
            }
            result["importablePhotos"] = array
        }
        
        return result
    }
}

class ImportableItem : NSObject, JsonProtocol {
    internal private(set) var filename: String?
    internal private(set) var fullPath: String?
    internal private(set) var thumbUrl: String?
    
    init(filename: String, fullPath: String, thumbUrl: String) {
        self.filename = filename
        self.fullPath = fullPath
        self.thumbUrl = thumbUrl
    }
    
    required init(json: [String: AnyObject]) {
        self.filename = json["filename"] as? String
        self.fullPath = json["fullPath"] as? String
        self.thumbUrl = json["thumbUrl"] as? String
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let filename = self.filename {
            result["filename"] = filename
        }
        if let fullPath = self.fullPath {
            result["fullPath"] = fullPath
        }
        if let thumbUrl = self.thumbUrl {
            result["thumbUrl"] = thumbUrl
        }
        
        return result
    }
}
