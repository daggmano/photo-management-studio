//
//  Db.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 6/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class Db: NSObject {
    
    struct MediaResponseObject {
        var media: [MediaObject]!
        
        init(rows: [CouchDb.View.Row]) {
            media = []
            for row in rows {
                if let doc = row.value as? [String: AnyObject] {
                    media.append(MediaObject(json: doc))
                }
            }
        }
    }
    
    enum MediaResponse {
        case Success(MediaResponseObject)
        case Error(NSError)
    }
    
    func getAllPhotos(done: (MediaResponse) -> Void) {
        
        // Check if connected, if so use server DB, otherwise use local DB
        let remoteDatabaseAddress = AppDelegate.getInstance()?.getRemoteDatabaseAddress()
        let localName = NSUserDefaults.standardUserDefaults().objectForKey("CouchDbLocalName") as? String
        
        print("Getting all photos, remoteDatabaseAddress: \(remoteDatabaseAddress), localName: \(localName)")

        var couchDb: CouchDb
        var database: CouchDb.Database? = nil
        if remoteDatabaseAddress != nil {
            couchDb = CouchDb(url: "http://\(remoteDatabaseAddress!):5984", name: nil, password: nil)
            database = couchDb.use("photos")
        } else if localName != nil {
            couchDb = CouchDb(url: "http://localhost:5984", name: nil, password: nil)
            database = couchDb.use(localName!)
        } else {
            done(.Error(NSError(domain: "DB", code: 0, userInfo: nil)))
        }
        
        if let database = database {
            let view = database.view("media")
            let params = CouchDb.QueryParameters()
            view.get("all", query: params) { response in
                switch response {
                case .Error(let error):
                    done(.Error(error))
                case .Success(let response):
                    let mediaResponseObject = MediaResponseObject(rows: response.rows)
                    done(.Success(mediaResponseObject))
                }
            }
        } else {
            done(.Error(NSError(domain: "DB", code: 0, userInfo: nil)))
        }
    }
}
