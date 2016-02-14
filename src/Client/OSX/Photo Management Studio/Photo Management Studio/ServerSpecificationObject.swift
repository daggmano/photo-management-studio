//
//  ServerSpecificationObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class ServerSpecificationObject : JsonProtocol {
    var _serverAddress: String?
    var _serverPort: UInt16?
    
    init(serverAddress: String, serverPort: UInt16) {
        _serverAddress = serverAddress
        _serverPort = serverPort
    }

    required init(json: [String: AnyObject]) {
        _serverAddress = json["serverAddress"] as? String
        if let serverPort = json["serverPort"] as? Int {
            _serverPort = UInt16(serverPort)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        result["serverAddress"] = _serverAddress
        if let serverPort = _serverPort {
            result["serverPort"] = Int(serverPort)
        }
        
        return result
    }
    
    func serverAddress() -> String? {
        return _serverAddress
    }
    
    func serverPort() -> UInt16? {
        return _serverPort
    }
}
