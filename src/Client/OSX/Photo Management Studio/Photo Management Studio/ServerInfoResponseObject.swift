//
//  ServerInfoResponseObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class ServerInfoResponseObject : ResponseObject<ServerDatabaseIdentifierObject> {
    required init(json: [String : AnyObject]) {
        super.init(json: json)
    }
}

class ServerDatabaseIdentifierObject : JsonProtocol {
    var _serverId: String?
    var _serverName: String?
    
    required init(json: [String: AnyObject]) {
        _serverId = json["serverId"] as? String
        _serverName = json["serverName"] as? String
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        result["serverId"] = _serverId
        result["serverName"] = _serverName
        
        return result
    }
}
