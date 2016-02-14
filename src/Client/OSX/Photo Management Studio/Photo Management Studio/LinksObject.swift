//
//  LinksObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class LinksObject : JsonProtocol {
    var _self: String?
    var _next: String?
    var _prev: String?
    
    init(myself: String, next: String, prev: String) {
        _self = myself
        _next = next
        _prev = prev
    }
    
    required init(json: [String: AnyObject]) {
        _self = json["self"] as? String
        _next = json["next"] as? String
        _prev = json["prev"] as? String
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let myself = _self {
            result["self"] = myself
        }
        if let next = _next {
            result["next"] = next
        }
        if let prev = _prev {
            result["prev"] = prev
        }
        
        return result
    }
}
