//
//  PingResponseObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class PingResponseObject : ResponseObject<PingResponseData> {}

class PingResponseData : JsonProtocol {
    var _serverDateTime: NSDate?

    init(serverDateTime: NSDate) {
        _serverDateTime = serverDateTime
    }
    
    required init(json: [String: AnyObject]) {
        if let serverDateTime = json["serverDateTime"] as? String {
            _serverDateTime = NSDate.dateFromISOString(serverDateTime)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let serverDateTime = _serverDateTime {
            result["serverDateTime"] = NSDate.ISOStringFromDate(serverDateTime)
        }
        
        return result
    }
}
