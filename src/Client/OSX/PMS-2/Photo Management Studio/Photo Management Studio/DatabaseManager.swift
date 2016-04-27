//
//  DatabaseManager.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 1/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Foundation

public class DatabaseManager {
    public static func setupReplication(remoteAddress: String, serverId: String) {
        let localName = "photos_\(serverId)"
        NSUserDefaults.standardUserDefaults().setObject(localName, forKey: "CouchDbLocalName")
        Event.emit("local-database-changed", obj: NSObject())
        
        // Need to test if local DB exists, and create if not
        
        let couchDb = CouchDb(url: "http://localhost:5984", name: nil, password: nil)
        
        couchDb.list { listResponse in
            switch listResponse {
            case .Error(let error):
                print(error)
                
            case .Success(let data):
                if (!data.databases.contains(localName)) {
                    couchDb.createDatabase(localName, done: { createDatabaseResponse in
                        switch createDatabaseResponse {
                        case .Error(let error):
                            print(error)
                        case .Success:
                            setupReplicationActual(remoteAddress, serverId: serverId, couchDb: couchDb)
                        }
                    })
                } else {
                    setupReplicationActual(remoteAddress, serverId: serverId, couchDb: couchDb)
                }
            }
        }
    }
    
    private static func setupReplicationActual(remoteAddress: String, serverId: String, couchDb: CouchDb) {
        let remote = "http://\(remoteAddress):5984/photos"
        let localName = "photos_\(serverId)"
        let local = "http://localhost:5984/\(localName)"
        
        AppDelegate.getInstance()?.setRemoteDatabaseAddress(remoteAddress);
        
        // Need to test if local DB exists, and create if not
        
        let remoteToLocalName = "repPhotosR2L_\(serverId)"
        let localToRemoteName = "repPhotosL2R_\(serverId)"
        
        var request1 = CouchDb.ReplicationRequest(replicationId: remoteToLocalName, source: remote, target: local)
        request1.continuous = true
        var request2 = CouchDb.ReplicationRequest(replicationId: localToRemoteName, source: local, target: remote)
        request2.continuous = true
        
        couchDb.replicate(request1) { response in
            switch response {
            case .Error(let error):
                print(error)
            case .Success:
                print("OK")
            }
        }
        couchDb.replicate(request2) { response in
            switch response {
            case .Error(let error):
                print(error)
            case .Success:
                print("OK")
            }
        }
    }
}
