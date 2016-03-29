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
    case Separator = "Separator"
}

class LibraryItem: NSObject, NSCoding {

    let text: String?
    let type: LibraryItemType!
    var children: [LibraryItem] = []

    init(asSeparator: Bool) {
        type = .Separator
        text = nil
    }
    
    init(asTitle: String) {
        type = .Title
        text = asTitle
    }
    
    init(asItem: String) {
        type = .Item
        text = asItem
    }
    
    init(type: LibraryItemType, text: String?, children: [LibraryItem]) {
        self.type = type
        self.text = text
        self.children = children
    }
    
    func isLeaf() -> Bool {
        return children.isEmpty
    }
    
    func isSeparator() -> Bool {
        return type == .Separator
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
            children: children
        )
    }
    
    func encodeWithCoder(coder: NSCoder) {
        coder.encodeObject(self.type.rawValue, forKey: "type")
        coder.encodeObject(self.text, forKey: "text")
        coder.encodeObject(self.children, forKey: "children")
    }
}
