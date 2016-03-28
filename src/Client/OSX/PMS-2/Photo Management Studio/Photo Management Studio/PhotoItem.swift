//
//  PhotoItem.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 28/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class PhotoItem: NSObject {

    let imageUrl: String!
    let fileName: String!
    let dimensions: String!
    
    init(fileName: String, dimensions: String, imageUrl: String) {
        self.fileName = fileName
        self.dimensions = dimensions
        self.imageUrl = imageUrl
    }
}
