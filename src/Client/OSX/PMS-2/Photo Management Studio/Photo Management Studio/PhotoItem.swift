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
    
    init(title: String, subTitle: String?, imageUrl: String, identifier: String?) {
        self.title = title
        self.subTitle = subTitle
        self.imageUrl = imageUrl
        self.identifier = identifier
    }
    
    // MARK: NSCoding
    
    required convenience init?(coder decoder: NSCoder) {
        guard let title = decoder.decodeObjectForKey("title") as? String,
            let imageUrl = decoder.decodeObjectForKey("imageUrl") as? String
            else { return nil }
        
        self.init(
            title: title,
            subTitle: decoder.decodeObjectForKey("siubTitle") as? String,
            imageUrl: imageUrl,
            identifier: decoder.decodeObjectForKey("identifier") as? String
        )
    }
    
    func encodeWithCoder(coder: NSCoder) {
        coder.encodeObject(self.title, forKey: "title")
        coder.encodeObject(self.subTitle, forKey: "subTitle")
        coder.encodeObject(self.imageUrl, forKey: "imageUrl")
        coder.encodeObject(self.identifier, forKey: "identifier")
    }
}
