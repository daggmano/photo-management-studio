//
//  ResponseObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 12/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

class ResponseObject<T : JsonProtocol> : JsonProtocol {
    var _links: LinksObject?
    var _data: T?
    
    init(links: LinksObject, data: T) {
        _links = links
        _data = data
    }
    
    required init(json: [String: AnyObject]) {
        if let links = json["links"] as? [String: AnyObject] {
            _links = LinksObject(json: links)
        }
        if let data = json["data"] as? [String: AnyObject] {
            _data = T(json: data)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let links = _links {
            result["links"] = links.toJSON()
        }
        if let data = _data {
            result["data"] = data.toJSON()
        }
        
        return result
    }
    
    func links() -> LinksObject? {
        return _links
    }
    
    func data() -> T? {
        return _data
    }
}

class ResponseListObject<T : JsonProtocol> : JsonProtocol {
    var _links: LinksObject?
    var _data: Array<T>?
    
    init(links: LinksObject, data: Array<T>) {
        _links = links
        _data = data
    }
    
    required init(json: [String: AnyObject]) {
        if let links = json["links"] as? [String: AnyObject] {
            _links = LinksObject(json: links)
        }
        if let dataArray = json["data"] as? [[String: AnyObject]] {
            _data = [T]()
            for data in dataArray {
                _data!.append(T(json: data))
            }
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()

        if let links = _links {
            result["links"] = links.toJSON()
        }
        if let data = _data {
            var array = [[String: AnyObject]]()
            for dataItem in data {
                array.append(dataItem.toJSON())
            }
            result["data"] = array
        }
        
        return result
    }
}
