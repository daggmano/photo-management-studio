//
//  CollectionView.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 27/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Cocoa

class CollectionViewController: NSViewController, NSCollectionViewDelegate {
    
    @IBOutlet weak var collectionView: NSCollectionView!
    var photoItems: [PhotoItem] = []

    required init?(coder: NSCoder) {
        super.init(coder: coder)
    }
    
    override init?(nibName nibNameOrNil: String?, bundle nibBundleOrNil: NSBundle?) {
        super.init(nibName: nibNameOrNil, bundle: nibBundleOrNil)
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        let nib = NSNib(nibNamed: "CollectionViewItem", bundle: nil)!
        collectionView.registerNib(nib, forItemWithIdentifier: "")
        
        self.willChangeValueForKey("photoItems")
        
        photoItems.append(PhotoItem(fileName: "FileName1", dimensions: "1920x1080", imageUrl: "Photo1"))
        photoItems.append(PhotoItem(fileName: "FileName2", dimensions: "1920x1080", imageUrl: "Photo2"))
        photoItems.append(PhotoItem(fileName: "FileName3", dimensions: "1920x1080", imageUrl: "Photo3"))
        photoItems.append(PhotoItem(fileName: "FileName4", dimensions: "1920x1080", imageUrl: "Photo4"))
        photoItems.append(PhotoItem(fileName: "FileName5", dimensions: "1920x1080", imageUrl: "Photo5"))
        photoItems.append(PhotoItem(fileName: "FileName6", dimensions: "1920x1080", imageUrl: "Photo6"))
        photoItems.append(PhotoItem(fileName: "FileName7", dimensions: "1920x1080", imageUrl: "Photo7"))
        photoItems.append(PhotoItem(fileName: "FileName8", dimensions: "1920x1080", imageUrl: "Photo8"))
        photoItems.append(PhotoItem(fileName: "FileName9", dimensions: "1920x1080", imageUrl: "Photo9"))
        photoItems.append(PhotoItem(fileName: "FileName10", dimensions: "1920x1080", imageUrl: "Photo10"))
        photoItems.append(PhotoItem(fileName: "FileName11", dimensions: "1920x1080", imageUrl: "Photo11"))
        photoItems.append(PhotoItem(fileName: "FileName12", dimensions: "1920x1080", imageUrl: "Photo12"))
        photoItems.append(PhotoItem(fileName: "FileName13", dimensions: "1920x1080", imageUrl: "Photo13"))
        
        self.didChangeValueForKey("photoItems")
    }
}
