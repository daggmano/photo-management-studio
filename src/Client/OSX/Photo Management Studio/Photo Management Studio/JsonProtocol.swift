//
//  JsonProtocol.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 13/02/2016.
//  Copyright © 2016 Darren Oster. All rights reserved.
//

import Foundation

protocol JsonProtocol {
    init(json: [String: AnyObject])
    func toJSON() -> [String: AnyObject]
}
