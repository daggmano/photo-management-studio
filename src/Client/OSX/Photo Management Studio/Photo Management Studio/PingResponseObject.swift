//
//  PingResponseObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class PingResponseObject : ResponseObject<PingResponseData> {
    required init(json: [String : AnyObject]) {
        super.init(json: json)
    }
}

class PingResponseData : JsonProtocol {
    var _serverDateTime: String?

    init(serverDateTime: String) {
        _serverDateTime = serverDateTime
    }
    
    required init(json: [String: AnyObject]) {
        _serverDateTime = json["serverDateTime"] as? String
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let serverDateTime = _serverDateTime {
            result["serverDateTime"] = serverDateTime
        }
        
        return result
    }
    
    func serverDateTime() -> String? {
        return _serverDateTime
    }
}
