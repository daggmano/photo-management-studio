//
//  PreviewWindow.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 7/03/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Cocoa

class PreviewWindowController: NSWindowController {
    
    @IBOutlet weak var imageView: NSImageView!
    @IBOutlet weak var progressBar: NSProgressIndicator!
    
    required init?(coder: NSCoder) {
        super.init(coder: coder)
    }
    
    override init(window: NSWindow?) {
        super.init(window: window)
    }

    override func windowDidLoad() {
        super.windowDidLoad()
        
        self.window?.level = Int(CGWindowLevelForKey(CGWindowLevelKey.StatusWindowLevelKey))
    }
    
    func setImageUrl(imageUrl: String) {
        var frame = self.window!.frame
        let targetSize = NSSize(width: 150, height: 150)
        
        frame.origin.y += frame.size.height // origin.y is top Y coordinate now
        frame.origin.y -= targetSize.height // new Y coordinate for the origin
        frame.size = targetSize
        
        self.window?.setFrame(frame, display: true, animate: false)
        self.imageView?.image = NSImage(named: "placeholder")
        self.progressBar.hidden = false
        self.progressBar.startAnimation(self)
        
        let mainScreenFrame = NSScreen.mainScreen()?.visibleFrame
        
        var url = imageUrl
        if let s = mainScreenFrame?.size {
            url = "\(imageUrl)&w=\(Int(s.width))&h=\(Int(s.height))"
        }
        
        dispatch_async(dispatch_queue_create("getAsyncPreviewGDQueue", nil), { () -> Void in

            if let url = NSURL(string: url) {
                if let image = NSImage(contentsOfURL: url) {
                    dispatch_async(dispatch_get_main_queue(), { () -> Void in
                        
                        let imageReps = image.representations
                        
                        var width = 0
                        var height = 0
                        
                        for imageRep in imageReps {
                            if (imageRep.pixelsWide > width) {
                                width = imageRep.pixelsWide
                            }
                            if (imageRep.pixelsHigh > height) {
                                height = imageRep.pixelsHigh
                            }
                        }
                        
                        if (height > Int((mainScreenFrame?.height)!)) {
                            width = width * Int((mainScreenFrame?.height)!) / height
                            height = Int((mainScreenFrame?.height)!)
                        }
                        
                        if (width > Int((mainScreenFrame?.width)!)) {
                            height = height * Int((mainScreenFrame?.width)!) / width
                            width = Int((mainScreenFrame?.width)!)
                        }
                        
                        print("Image is \(width)x\(height)")
                        let imageSize = NSSize(width: width, height: height)
                        
                        frame.origin.y += frame.size.height
                        frame.origin.y -= imageSize.height
                        frame.size = imageSize
                        
                        self.window?.constrainFrameRect(frame, toScreen: NSScreen.mainScreen())
                        if (frame.origin.x < mainScreenFrame!.origin.x) {
                            frame.origin.x = mainScreenFrame!.origin.x
                        }
                        
                        if (frame.origin.y < mainScreenFrame!.origin.y) {
                            frame.origin.y = mainScreenFrame!.origin.y
                        }
                        
                        if ((frame.origin.x + frame.size.width) > (mainScreenFrame!.origin.x + mainScreenFrame!.size.width)) {
                            frame.origin.x = mainScreenFrame!.origin.x
                        }
                        
                        self.imageView?.image = image
                        self.imageView?.needsDisplay = true
                        self.progressBar.hidden = true
                        self.progressBar.stopAnimation(self)
                        
                        self.window?.setFrame(frame, display: true, animate: true)
                        self.window?.aspectRatio = imageSize
                    })
                }
            }
        })
    }
}
