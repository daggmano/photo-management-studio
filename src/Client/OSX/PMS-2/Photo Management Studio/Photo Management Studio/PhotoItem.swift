//
//  PhotoItem.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 28/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class PhotoItem: NSObject, NSCoding {

    let title: String
    let subTitle: String?
    let imageUrl: String
    let identifier: String?
    let meta: [MetaItem]?
    
    init(title: String, subTitle: String?, imageUrl: String, identifier: String?, metadata: [MetadataObject]?) {
        self.title = title
        self.subTitle = subTitle
        self.imageUrl = imageUrl
        self.identifier = identifier
        self.meta = []
        
        if let metadata = metadata {
            for md in metadata {
                self.meta?.append(MetaItem(metadata: md))
            }
        }
    }
    
    // MARK: NSCoding
    
    required convenience init?(coder decoder: NSCoder) {
        guard let title = decoder.decodeObjectForKey("title") as? String,
            let imageUrl = decoder.decodeObjectForKey("imageUrl") as? String
            else { return nil }
        
        self.init(
            title: title,
            subTitle: decoder.decodeObjectForKey("subTitle") as? String,
            imageUrl: imageUrl,
            identifier: decoder.decodeObjectForKey("identifier") as? String,
            metadata: nil
        )
    }
    
    func encodeWithCoder(coder: NSCoder) {
        coder.encodeObject(self.title, forKey: "title")
        coder.encodeObject(self.subTitle, forKey: "subTitle")
        coder.encodeObject(self.imageUrl, forKey: "imageUrl")
        coder.encodeObject(self.identifier, forKey: "identifier")
    }
}

class MetaItem: NSObject, NSCoding {
    let title: String
    let value: String
    
    init(metadata: MetadataObject) {
        var t = ""
        if let group = metadata.group {
            t = group
        }
        if let name = metadata.name {
            if (t.characters.count > 0) {
                t += ".\(name)"
            } else {
                t = name
            }
        }
        self.title = t
        self.value = metadata.value ?? ""
    }
    
    init(title: String, value: String) {
        self.title = title
        self.value = value
    }
    
    // MARK: NSCoding
    
    required convenience init?(coder decoder: NSCoder) {
        guard let title = decoder.decodeObjectForKey("title") as? String,
            let value = decoder.decodeObjectForKey("value") as? String
            else { return nil }
        
        self.init(
            title: title,
            value: value
        )
    }
    
    func encodeWithCoder(coder: NSCoder) {
        coder.encodeObject(self.title, forKey: "title")
        coder.encodeObject(self.value, forKey: "value")
    }
}
