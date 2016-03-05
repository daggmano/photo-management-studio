//
//  NetworkDiscoveryObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 11/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class NetworkDiscoveryObject : NSObject, JsonProtocol {
    internal private(set) var identifier: String?
    internal private(set) var clientSocketPort: UInt16?
    
    init(identifier: String, clientSocketPort: UInt16) {
        self.identifier = identifier
        self.clientSocketPort = clientSocketPort
    }
    
    required init(json: [String: AnyObject]) {
        self.identifier = json["identifier"] as? String
        if let clientSocketPort = json["clientSocketPort"] as? Int {
            self.clientSocketPort = UInt16(clientSocketPort)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let identifier = self.identifier {
            result["identifier"] = identifier
        }
        if let clientSocketPort = self.clientSocketPort {
            result["clientSocketPort"] = Int(clientSocketPort)
        }
        
        return result
    }
}
