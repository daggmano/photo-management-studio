//
//  ImportPhotosRequestObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class ImportPhotosRequestObject : NSObject, JsonProtocol {
    internal private(set) var photoPaths: [String]?
    
    init(photoPaths: [String]) {
        self.photoPaths = photoPaths
    }
    
    required init(json: [String: AnyObject]) {
        self.photoPaths = json["photoPaths"] as? [String]
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let photoPaths = self.photoPaths {
            result["photoPaths"] = photoPaths
        }
        
        return result
    }
}
