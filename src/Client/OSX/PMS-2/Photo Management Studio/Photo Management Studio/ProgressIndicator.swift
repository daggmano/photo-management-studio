//
//  ProgressIndicator.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 5/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import ITProgressIndicator

class ProgressIndicator: NSObject {
    
    let _progressIndicator: ITProgressIndicator!
    
    init(progressIndicator: ITProgressIndicator) {
        _progressIndicator = progressIndicator
        
        _progressIndicator.isIndeterminate = true
        _progressIndicator.lengthOfLine = 30
        _progressIndicator.widthOfLine = 5
        _progressIndicator.numberOfLines = 10
        _progressIndicator.color = NSColor.lightGrayColor()
        _progressIndicator.steppedAnimation = true
        _progressIndicator.progress = 1
        _progressIndicator.animationDuration = 1
    }
    
    func show(makeVisible: Bool) {
        dispatch_async(dispatch_get_main_queue()) { () -> Void in
            self._progressIndicator.hidden = !makeVisible
        }
    }
}
