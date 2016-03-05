//
//  ResponseObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class ResponseObject<T : JsonProtocol> : NSObject, JsonProtocol {
    internal private(set) var links: LinksObject?
    internal private(set) var data: T?
    
    init(links: LinksObject, data: T) {
        self.links = links
        self.data = data
    }
    
    required init(json: [String: AnyObject]) {
        if let links = json["links"] as? [String: AnyObject] {
            self.links = LinksObject(json: links)
        }
        if let data = json["data"] as? [String: AnyObject] {
            self.data = T(json: data)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let links = self.links {
            result["links"] = links.toJSON()
        }
        if let data = self.data {
            result["data"] = data.toJSON()
        }
        
        return result
    }
}

class ResponseListObject<T : JsonProtocol> : NSObject, JsonProtocol {
    internal private(set) var links: LinksObject?
    internal private(set) var data: Array<T>?
    
    init(links: LinksObject, data: Array<T>) {
        self.links = links
        self.data = data
    }
    
    required init(json: [String: AnyObject]) {
        if let links = json["links"] as? [String: AnyObject] {
            self.links = LinksObject(json: links)
        }
        if let dataArray = json["data"] as? [[String: AnyObject]] {
            self.data = [T]()
            for data in dataArray {
                self.data!.append(T(json: data))
            }
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let links = self.links {
            result["links"] = links.toJSON()
        }
        if let data = self.data {
            var array = [[String: AnyObject]]()
            for dataItem in data {
                array.append(dataItem.toJSON())
            }
            result["data"] = array
        }
        
        return result
    }
}
