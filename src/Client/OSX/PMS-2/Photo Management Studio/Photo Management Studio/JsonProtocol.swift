//
//  JsonProtocol.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 31/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

protocol JsonProtocol {
    init(json: [String: AnyObject])
    func toJSON() -> [String: AnyObject]
}
