//
//  InspectorItem.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 21/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

enum InspectorItemType: String {
    case Title = "Title"
    case Item = "Item"
}

class InspectorItem: NSObject {
    
    let title: String
    let value: String
    let type: InspectorItemType
    var children: [InspectorItem] = []
    
    init(asTitle: String) {
        self.type = .Title
        self.title = asTitle
        self.value = ""
    }
    
    init(asItem: String, withValue: String) {
        self.type = .Item
        self.title = asItem
        self.value = withValue
    }
    
    func isLeaf() -> Bool {
        return children.isEmpty
    }
    
    func isTitle() -> Bool {
        return type == .Title
    }
}
