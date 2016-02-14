//
//  Config.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 7/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

struct Config {
    static let couchDbPath = "http://localhost:5984/"
    static let cacheFolder = "PhotoLibraryCache"
    static let udpSearchPort: UInt16 = 11000
    static let watchdogTimeout = 5000
    
    static let rollbarAccessToken = "[ INSERT ROLLBAR API KEY HERE ]"
    static let rollbarEnvironment = "development"
}
