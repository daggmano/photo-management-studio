//
//  ImportedFilesObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 15/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class ImportedFilesObject : NSObject, JsonProtocol {
    internal private(set) var itemCount: Int?
    internal private(set) var importedPhotos: [String]?
    
    init(itemCount: Int, importedPhotos: [String]) {
        self.itemCount = itemCount
        self.importedPhotos = importedPhotos
    }
    
    required init(json: [String: AnyObject]) {
        self.itemCount = json["itemCount"] as? Int
        self.importedPhotos = json["importedPhotos"] as? [String]
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let itemCount = self.itemCount {
            result["itemCount"] = itemCount
        }
        if let importedPhotos = self.importedPhotos {
            result["importedPhotos"] = importedPhotos
        }
        
        return result
    }
}
