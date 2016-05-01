//
//  TagObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 30/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class TagObject: NSObject, JsonProtocol {
    internal private(set) var tagId: String?
    internal private(set) var tagRev: String?
    internal private(set) var subType: String?
    internal private(set) var name: String?
    internal private(set) var color: String?
    internal private(set) var parentId: String?
    
    init(tagId: String, tagRev: String, subType: String, name: String, color: String?, parentId: String?) {
        self.tagId = tagId
        self.tagRev = tagRev
        self.subType = subType
        self.name = name
        self.color = color
        self.parentId = parentId
    }
    
    required init(json: [String: AnyObject]) {
        self.tagId = json["_id"] as? String
        self.tagRev = json["_rev"] as? String
        self.subType = json["subType"] as? String
        self.name = json["name"] as? String
        self.color = json["color"] as? String
        self.parentId = json["parentId"] as? String
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let tagId = self.tagId {
            result["_id"] = tagId
        }
        if let tagRev = self.tagRev {
            result["_rev"] = tagRev
        }
        if let subType = self.subType {
            result["subType"] = subType
        }
        if let name = self.name {
            result["name"] = name
        }
        if let color = self.color {
            result["color"] = color
        }
        if let parentId = self.parentId {
            result["parentId"] = parentId
        }
        
        return result
    }
}
