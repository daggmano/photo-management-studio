//
//  LibraryItem.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 21/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

enum LibraryItemType: String {
    case Title = "Title"
    case Item = "Item"
}

class LibraryItem: NSObject, NSCoding {

    let text: String
    let type: LibraryItemType
    let canTag: Bool
    var children: [LibraryItem] = []

    init(asTitle: String) {
        self.type = .Title
        self.text = asTitle
        self.canTag = false
    }
    
    init(asItem: String, canTag: Bool) {
        self.type = .Item
        self.text = asItem
        self.canTag = canTag
    }
    
    init(type: LibraryItemType, text: String, canTag: Bool, children: [LibraryItem]) {
        self.type = type
        self.text = text
        self.canTag = canTag
        self.children = children
    }
    
    func isLeaf() -> Bool {
        return children.isEmpty
    }
    
    func isTitle() -> Bool {
        return type == .Title
    }
    
    // MARK: NSCoding
    
    required convenience init?(coder decoder: NSCoder) {
        guard let text = decoder.decodeObjectForKey("text") as? String,
            let type = LibraryItemType(rawValue: (decoder.decodeObjectForKey("type") as! String)),
            let children = decoder.decodeObjectForKey("children") as? [LibraryItem]
            else { return nil }
        
        self.init(
            type: type,
            text: text,
            canTag: decoder.decodeBoolForKey("canTag"),
            children: children
        )
    }
    
    func encodeWithCoder(coder: NSCoder) {
        coder.encodeObject(self.type.rawValue, forKey: "type")
        coder.encodeObject(self.text, forKey: "text")
        coder.encodeBool(self.canTag, forKey: "canTag")
        coder.encodeObject(self.children, forKey: "children")
    }
}
