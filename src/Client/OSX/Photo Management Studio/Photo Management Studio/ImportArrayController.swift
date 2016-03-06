//
//  ImportArrayController.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 6/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class ImportArrayController: NSArrayController {
    override func arrangeObjects(objects: [AnyObject]) -> [AnyObject] {
        var returnValue = objects
        let sortDescriptors = self.sortDescriptors
        
        if (sortDescriptors.count > 0) {
            returnValue = (objects as NSArray).sortedArrayUsingDescriptors(sortDescriptors)
        } else {
            returnValue = super.arrangeObjects(objects)
        }
        
        return returnValue
    }
}
