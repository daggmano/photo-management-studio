//
//  DatabaseManager.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 20/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

public class DatabaseManager {
    public static func setupReplication(remoteAddress: String, serverId: String) {
        let localName = "photos_\(serverId)"
        
        // Need to test if local DB exists, and create if not
        
        let couchDb = CouchDB(url: "http://localhost:5984", name: nil, password: nil)
        
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
    
    private static func setupReplicationActual(remoteAddress: String, serverId: String, couchDb: CouchDB) {
        let remote = "http://\(remoteAddress):5984/photos"
        let localName = "photos_\(serverId)"
        let local = "http://localhost:5984/\(localName)"
        
        // Need to test if local DB exists, and create if not
        
        let remoteToLocalName = "repPhotosR2L_\(serverId)"
        let localToRemoteName = "repPhotosL2R_\(serverId)"
        
        var request1 = CouchDB.ReplicationRequest(replicationId: remoteToLocalName, source: remote, target: local)
        request1.continuous = true
        var request2 = CouchDB.ReplicationRequest(replicationId: localToRemoteName, source: local, target: remote)
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
/*

var request1 = new ReplicateDatabaseRequest(remoteToLocalName, remote, local)
{
Continuous = true,
CreateTarget = true
};

var request2 = new ReplicateDatabaseRequest(localToRemoteName, local, remote)
{
Continuous = true,
CreateTarget = false
};

try
{
await client.Replicator.ReplicateAsync(request1);
await client.Replicator.ReplicateAsync(request2);
}
catch (Exception ex)
{
Debug.WriteLine("Exception: " + ex.Message);
ErrorReporter.SendException(ex);
}
}

}
}
}

*/