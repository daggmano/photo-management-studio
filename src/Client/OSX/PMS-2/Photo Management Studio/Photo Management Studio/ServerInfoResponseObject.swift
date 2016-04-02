//
//  ServerInfoResponseObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 1/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Foundation

class ServerInfoResponseObject : ResponseObject<ServerDatabaseIdentifierObject> {
    required init(json: [String : AnyObject]) {
        super.init(json: json)
    }
}

class ServerDatabaseIdentifierObject : NSObject, JsonProtocol {
    internal private(set) var serverId: String?
    internal private(set) var serverName: String?
    
    required init(json: [String: AnyObject]) {
        self.serverId = json["serverId"] as? String
        self.serverName = json["serverName"] as? String
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        result["serverId"] = self.serverId
        result["serverName"] = self.serverName
        
        return result
    }
}
