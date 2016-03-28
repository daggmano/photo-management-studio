//
//  HomeButton.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 15/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class PMSHoverButton: NSButton {
    
    var imageTmp: NSImage!

    override func mouseEntered(theEvent: NSEvent) {
        super.mouseEntered(theEvent)
        
        self.updateImages()
        self.image = self.alternateImage
    }

    override func mouseExited(theEvent: NSEvent) {
        super.mouseExited(theEvent)
        
        self.image = imageTmp
    }
    
    private func updateImages() {
        self.imageTmp = self.image
    }
    
    override func updateTrackingAreas() {
        self.addTrackingRect(self.bounds, owner: self, userData: nil, assumeInside: true)
    }
}
