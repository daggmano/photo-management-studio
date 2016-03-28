//
//  PMSCollectionView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 17/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class PMSCollectionView: NSCollectionView {

    override func drawRect(dirtyRect: NSRect) {
        
        NSColor(red: 0.2, green: 0.2, blue: 0.2, alpha: 1.0).setFill()
        NSRectFill(dirtyRect)
        
    }
    
}
