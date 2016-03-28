//
//  LibraryItem.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 21/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

enum LibraryItemType {
    case Title, Item, Separator
}

class LibraryItem: NSObject {

    let text: String?
    let image: NSImage?
    let type: LibraryItemType!
    var children: [LibraryItem] = []

    init(asSeparator: Bool) {
        type = .Separator
        text = nil
        image = nil
    }
    
    init(asTitle: String) {
        type = .Title
        text = asTitle
        image = nil
    }
    
    init(asItem: String) {
        type = .Item
        text = asItem
        image = nil
    }
    
    init(asItem: String, withImage: NSImage) {
        type = .Item
        text = asItem
        image = withImage
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
}
