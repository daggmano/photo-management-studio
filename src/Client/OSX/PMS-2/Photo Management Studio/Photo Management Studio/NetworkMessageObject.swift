//
//  NetworkMessageObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 1/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Foundation

enum NetworkMessageType: Int {
    case ServerSpecification
}

protocol INetworkMessageObject {
    var messageType: NetworkMessageType? { get }
}

class NetworkMessageObject : NSObject, INetworkMessageObject, JsonProtocol {
    internal private(set) var messageType: NetworkMessageType?
    
    init(messageType: NetworkMessageType) {
        self.messageType = messageType
    }
    
    required init(json: [String: AnyObject]) {
        if let messageType = json["messageType"] as? Int {
            self.messageType = NetworkMessageType(rawValue: messageType)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let messageType = self.messageType {
            result["messageType"] = messageType.rawValue
        }
        
        return result
    }
}

class NetworkMessageObjectGeneric<T : JsonProtocol> : NSObject, INetworkMessageObject, JsonProtocol {
    internal private(set) var messageType: NetworkMessageType?
    internal private(set) var message: T?
    
    init(messageType: NetworkMessageType, message: T) {
        self.messageType = messageType
        self.message = message
    }
    
    required init(json: [String: AnyObject]) {
        if let messageType = json["messageType"] as? Int {
            self.messageType = NetworkMessageType(rawValue: messageType)
        }
        if let message = json["message"] as? [String: AnyObject] {
            let m = T(json: message)
            print(m)
            self.message = T(json: message)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let messageType = self.messageType {
            result["messageType"] = messageType.rawValue
        }
        if let message = self.message {
            result["message"] = message.toJSON()
        }
        
        return result
    }
}
