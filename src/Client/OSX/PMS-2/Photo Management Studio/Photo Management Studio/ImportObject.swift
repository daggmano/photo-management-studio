//
//  ImportObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 30/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class ImportObject: NSObject, JsonProtocol {
    internal private(set) var importId: String?
    internal private(set) var importRev: String?
    internal private(set) var importDate: NSDate?
    
    // date format from http://userguide.icu-project.org/formatparse/datetime
    private let dateFormat = "yyyy-MM-dd'T'HH:mm:ss.SSSSSS'Z'"
    
    init(importId: String, importRev: String, importDate: NSDate) {
        self.importId = importId
        self.importRev = importRev
        self.importDate = importDate
    }
    
    required init(json: [String: AnyObject]) {
        self.importId = json["_id"] as? String
        self.importRev = json["_rev"] as? String
        
        if let importDateString = json["importDate"] as? String {
            // format is "2016-04-27T09:59:07.440636Z"
            let dateFormatter = NSDateFormatter()
            dateFormatter.dateFormat = dateFormat
            self.importDate = dateFormatter.dateFromString(importDateString)
        }
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let importId = self.importId {
            result["_id"] = importId
        }
        if let importRev = self.importRev {
            result["_rev"] = importRev
        }
        if let importDate = self.importDate {
            let dateFormatter = NSDateFormatter()
            dateFormatter.dateFormat = dateFormat
            result["importDate"] = dateFormatter.stringFromDate(importDate)
        }
        
        return result
    }
}
