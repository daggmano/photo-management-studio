//
//  JobResponseObject.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 3/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Foundation

class JobResponseObject : NSObject, JsonProtocol {
    internal private(set) var jobId: String?
    internal private(set) var status: String?
    
    init(jobId: String, status: String) {
        self.jobId = jobId
        self.status = status
    }
    
    required init(json: [String: AnyObject]) {
        self.jobId = json["jobId"] as? String
        self.status = json["status"] as? String
    }
    
    func toJSON() -> [String: AnyObject] {
        var result = [String: AnyObject]()
        
        if let jobId = self.jobId {
            result["jobId"] = jobId
        }
        if let status = self.status {
            result["status"] = status
        }
        
        return result
    }
}
