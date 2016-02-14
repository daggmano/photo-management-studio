//
//  ImportPhotosRequestObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class ImportPhotosRequestObject : JsonProtocol {
    var _photoPaths: [String]?
    
    init(photoPaths: [String]) {
        _photoPaths = photoPaths
    }
    
    required init(json: [String: AnyObject]) {
        _photoPaths = json["photoPaths"] as? [String]
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let photoPaths = _photoPaths {
            result["photoPaths"] = photoPaths
        }
        
        return result
    }
}
