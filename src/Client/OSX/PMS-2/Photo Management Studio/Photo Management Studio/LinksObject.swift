//
//  LinksObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 31/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class LinksObject : NSObject, JsonProtocol {
    internal private(set) var linkSelf: String?
    internal private(set) var linkNext: String?
    internal private(set) var linkPrev: String?
    
    init(myself: String, next: String, prev: String) {
        self.linkSelf = myself
        self.linkNext = next
        self.linkPrev = prev
    }
    
    required init(json: [String: AnyObject]) {
        self.linkSelf = json["self"] as? String
        self.linkNext = json["next"] as? String
        self.linkPrev = json["prev"] as? String
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let linkSelf = self.linkSelf {
            result["self"] = linkSelf
        }
        if let linkNext = self.linkNext {
            result["next"] = linkNext
        }
        if let linkPrev = self.linkPrev {
            result["prev"] = linkPrev
        }
        
        return result
    }
}
