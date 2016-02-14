//
//  ImportableListObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class ImportableListObject : JsonProtocol {
    var _itemCount: Int?
    var _importablePhotos: [ImportableItem]?
    
    init(itemCount: Int, importablePhotos: [ImportableItem]) {
        _itemCount = itemCount
        _importablePhotos = importablePhotos
    }
    
    required init(json: [String: AnyObject]) {
        _itemCount = json["itemCount"] as? Int
        if let importablePhotos = json["importablePhotos"] as? [[String: AnyObject]] {
            _importablePhotos = [ImportableItem]()
            
            for item in importablePhotos {
                _importablePhotos!.append(ImportableItem(json: item))
            }
         }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let itemCount = _itemCount {
            result["itemCount"] = itemCount
        }
        if let importablePhotos = _importablePhotos {
            var array = [[String: AnyObject]]()
            for item in importablePhotos {
                array.append(item.toJSON())
            }
            result["importablePhotos"] = array
        }
        
        return result
    }
}

class ImportableItem : JsonProtocol {
    var _filename: String?
    var _fullPath: String?
    var _thumbUrl: String?
    
    init(filename: String, fullPath: String, thumbUrl: String) {
        _filename = filename
        _fullPath = fullPath
        _thumbUrl = thumbUrl
    }
    
    required init(json: [String: AnyObject]) {
        _filename = json["filename"] as? String
        _fullPath = json["fullPath"] as? String
        _thumbUrl = json["thumbUrl"] as? String
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let filename = _filename {
            result["filename"] = filename
        }
        if let fullPath = _fullPath {
            result["fullPath"] = fullPath
        }
        if let thumbUrl = _thumbUrl {
            result["thumbUrl"] = thumbUrl
        }
        
        return result
    }
}
