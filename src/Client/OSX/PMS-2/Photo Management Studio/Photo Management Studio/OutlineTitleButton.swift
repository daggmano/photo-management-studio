//
//  OutlineTitleButton.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 26/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class OutlineTitleButton: NSButton {

    override func awakeFromNib() {
        let color = NSColor(deviceWhite: 0.85, alpha: 1.0)
        
        let colorTitle = NSMutableAttributedString(attributedString: self.attributedTitle)
        let titleRange = NSMakeRange(0, colorTitle.length)
        
        colorTitle.addAttribute(NSForegroundColorAttributeName, value: color, range: titleRange)
        attributedTitle = colorTitle
    }
    
}
