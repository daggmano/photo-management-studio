//
//  PingResponseObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 31/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class PingResponseObject : ResponseObject<PingResponseData> {
    required init(json: [String : AnyObject]) {
        super.init(json: json)
    }
}

class PingResponseData : NSObject, JsonProtocol {
    internal private(set) var serverDateTime: String?
    
    init(serverDateTime: String) {
        self.serverDateTime = serverDateTime
    }
    
    required init(json: [String: AnyObject]) {
        self.serverDateTime = json["serverDateTime"] as? String
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let serverDateTime = self.serverDateTime {
            result["serverDateTime"] = serverDateTime
        }
        
        return result
    }
}
