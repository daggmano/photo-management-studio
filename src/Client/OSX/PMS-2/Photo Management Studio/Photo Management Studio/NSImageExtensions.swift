//
//  NSImageExtensions.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 28/04/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

extension NSImage {
 
    func saveAsJpegWithPath(filePath: String) -> Void {
        // Cache the reduced image
        if let imageData = self.TIFFRepresentation {
            if let imageRep = NSBitmapImageRep(data: imageData) {
                let imageProps: [String: AnyObject] = [NSImageCompressionFactor: NSNumber(float: 1.0)]
                let jpegData = imageRep.representationUsingType(.NSJPEGFileType, properties: imageProps)
                
                do {
                    try jpegData?.writeToFile(filePath, options: .DataWritingWithoutOverwriting)
                } catch {}
            }
        }
    }
}
