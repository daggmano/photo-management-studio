//
//  MediaObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 6/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class MediaObject: NSObject, JsonProtocol {
    internal private(set) var mediaId: String?
    internal private(set) var mediaRev: String?
    internal private(set) var importId: String?
    internal private(set) var uniqueId: String?
    internal private(set) var fullFilePath: String?
    internal private(set) var fileName: String?
    internal private(set) var shotDate: String?
    internal private(set) var rating: Int?
    internal private(set) var dateAccuracy: Int?
    internal private(set) var caption: String?
    internal private(set) var rotate: Int?
    internal private(set) var metadata: [MetadataObject]?
    internal private(set) var tagIds: [String]?
    
    init(mediaId: String, mediaRev: String, importId: String, uniqueId: String, fullFilePath: String, fileName: String, shotDate: String, rating: Int, dateAccuracy: Int, caption: String, rotate: Int, metadata: [MetadataObject], tagIds: [String]) {
        self.mediaId = mediaId
        self.mediaRev = mediaRev
        self.importId = importId
        self.uniqueId = uniqueId
        self.fullFilePath = fullFilePath
        self.fileName = fileName
        self.shotDate = shotDate
        self.rating = rating
        self.dateAccuracy = dateAccuracy
        self.caption = caption
        self.rotate = rotate
        self.metadata = metadata
        self.tagIds = tagIds
    }
    
    required init(json: [String: AnyObject]) {
        self.mediaId = json["_id"] as? String
        self.mediaRev = json["_rev"] as? String
        self.importId = json["importId"] as? String
        self.uniqueId = json["uniqueId"] as? String
        self.fullFilePath = json["fullFilePath"] as? String
        self.fileName = json["fileName"] as? String
        self.shotDate = json["shotDate"] as? String
        self.rating = json["rating"] as? Int
        self.dateAccuracy = json["dateAccuracy"] as? Int
        self.caption = json["caption"] as? String
        self.rotate = json["rotate"] as? Int
        if let metadata = json["metadata"] as? [[String: AnyObject]] {
            self.metadata = [MetadataObject]()
            
            for item in metadata {
                self.metadata!.append(MetadataObject(json: item))
            }
        }
        self.tagIds = json["tagIds"] as? [String]
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let mediaId = self.mediaId {
            result["_id"] = mediaId
        }
        if let mediaRev = self.mediaRev {
            result["_rev"] = mediaRev
        }
        if let importId = self.importId {
            result["importId"] = importId
        }
        if let uniqueId = self.uniqueId {
            result["uniqueId"] = uniqueId
        }
        if let fullFilePath = self.fullFilePath {
            result["fullFilePath"] = fullFilePath
        }
        if let fileName = self.fileName {
            result["fileName"] = fileName
        }
        if let shotDate = self.shotDate {
            result["shotDate"] = shotDate
        }
        if let rating = self.rating {
            result["rating"] = rating
        }
        if let dateAccuracy = self.dateAccuracy {
            result["dateAccuracy"] = dateAccuracy
        }
        if let caption = self.caption {
            result["caption"] = caption
        }
        if let rotate = self.rotate {
            result["rotate"] = rotate
        }
        if let metadata = self.metadata {
            var array = [[String: AnyObject]]()
            for item in metadata {
                array.append(item.toJSON())
            }
            result["metadata"] = array
        }
        if let tagIds = self.tagIds {
            result["tagIds"] = tagIds
        }
        
        return result
    }
}

class MetadataObject: NSObject, JsonProtocol {
    internal private(set) var group: String?
    internal private(set) var name: String?
    internal private(set) var value: String?

    init(group: String, name: String, value: String) {
        self.group = group
        self.name = name
        self.value = value
    }
    
    required init(json: [String: AnyObject]) {
        self.group = json["group"] as? String
        self.name = json["name"] as? String
        self.value = json["value"] as? String
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let group = self.group {
            result["group"] = group
        }
        if let name = self.name {
            result["name"] = name
        }
        if let value = self.value {
            result["value"] = value
        }
        
        return result
    }
}
