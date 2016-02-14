//
//  NetworkDiscoveryObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 11/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class NetworkDiscoveryObject : JsonProtocol {
    var _identifier: String?
    var _clientSocketPort: UInt16?
    
    init(identifier: String, clientSocketPort: UInt16) {
        _identifier = identifier
        _clientSocketPort = clientSocketPort
    }
    
    required init(json: [String: AnyObject]) {
        _identifier = json["identifier"] as? String
        if let clientSocketPort = json["clientSocketPort"] as? Int {
            _clientSocketPort = UInt16(clientSocketPort)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let identifier = _identifier {
            result["identifier"] = identifier
        }
        if let clientSocketPort = _clientSocketPort {
            result["clientSocketPort"] = Int(clientSocketPort)
        }
        
        return result
    }
}
