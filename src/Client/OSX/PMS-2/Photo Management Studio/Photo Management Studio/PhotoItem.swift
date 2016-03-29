//
//  PhotoItem.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 28/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class PhotoItem: NSObject, NSCoding {

    let imageUrl: String!
    let fileName: String!
    let dimensions: String!
    
    init(fileName: String, dimensions: String, imageUrl: String) {
        self.fileName = fileName
        self.dimensions = dimensions
        self.imageUrl = imageUrl
    }
    
    // MARK: NSCoding
    
    required convenience init?(coder decoder: NSCoder) {
        guard let imageUrl = decoder.decodeObjectForKey("imageUrl") as? String,
            let fileName = decoder.decodeObjectForKey("fileName") as? String,
            let dimensions = decoder.decodeObjectForKey("dimensions") as? String
            else { return nil }
        
        self.init(
            fileName: fileName,
            dimensions: dimensions,
            imageUrl: imageUrl
        )
    }
    
    func encodeWithCoder(coder: NSCoder) {
        coder.encodeObject(self.fileName, forKey: "fileName")
        coder.encodeObject(self.dimensions, forKey: "dimensions")
        coder.encodeObject(self.imageUrl, forKey: "imageUrl")
    }
}
