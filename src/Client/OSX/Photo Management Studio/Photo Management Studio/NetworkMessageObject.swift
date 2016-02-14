//
//  NetworkMessageObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 11/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

enum NetworkMessageType: Int {
    case ServerSpecification
}

protocol INetworkMessageObject {
    var _messageType: NetworkMessageType? { get set }
}

class NetworkMessageObject : INetworkMessageObject, JsonProtocol {
    var _messageType: NetworkMessageType?
    
    init(messageType: NetworkMessageType) {
        _messageType = messageType
    }
    
    required init(json: [String: AnyObject]) {
        if let messageType = json["messageType"] as? Int {
            _messageType = NetworkMessageType(rawValue: messageType)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let messageType = _messageType {
            result["messageType"] = messageType.rawValue
        }
        
        return result
    }
    
    func messageType() -> NetworkMessageType? {
        return _messageType
    }
}

class NetworkMessageObjectGeneric<T : JsonProtocol> : INetworkMessageObject, JsonProtocol {
    var _messageType: NetworkMessageType?
    var _message: T?
    
    init(messageType: NetworkMessageType, message: T) {
        _messageType = messageType
        _message = message
    }
    
    required init(json: [String: AnyObject]) {
        if let messageType = json["messageType"] as? Int {
            _messageType = NetworkMessageType(rawValue: messageType)
        }
        if let message = json["message"] as? [String: AnyObject] {
            let m = T(json: message)
            print(m)
            _message = T(json: message)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let messageType = _messageType {
            result["messageType"] = messageType.rawValue
        }
        if let message = _message {
            result["message"] = message.toJSON()
        }
        
        return result
    }
    
    func messageType() -> NetworkMessageType? {
        return _messageType
    }
    
    func message() -> T? {
        return _message
    }
}
