//
//  ServerSpecificationObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class ServerSpecificationObject : NSObject, JsonProtocol {
    internal private(set) var serverAddress: String?
    internal private(set) var serverPort: UInt16?
    
    init(serverAddress: String, serverPort: UInt16) {
        self.serverAddress = serverAddress
        self.serverPort = serverPort
    }

    required init(json: [String: AnyObject]) {
        self.serverAddress = json["serverAddress"] as? String
        if let serverPort = json["serverPort"] as? Int {
            self.serverPort = UInt16(serverPort)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        result["serverAddress"] = self.serverAddress
        if let serverPort = self.serverPort {
            result["serverPort"] = Int(serverPort)
        }
        
        return result
    }
}
