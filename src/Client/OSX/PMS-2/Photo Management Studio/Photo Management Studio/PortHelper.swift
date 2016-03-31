//
//  PortHelper.swift
//  Photo Management Studio
//
//  Created by Darren Oster on 31/03/2016.
//  Copyright © 2016 Criterion Software. All rights reserved.
//

import Foundation

class PortHelper {

    static func availableTcpPort() -> Int32 {
        var server_addr = sockaddr_in()
        var server_addr_size = socklen_t(sizeofValue(server_addr))
        server_addr.sin_len = UInt8(server_addr_size)
        server_addr.sin_family = sa_family_t(AF_INET)
        server_addr.sin_port = 0
        
        inet_aton("0.0.0.0", &server_addr.sin_addr)
        
        let sock = socket(AF_INET, SOCK_STREAM, 0)
        if (sock < 0) {
            perror("socket()")
            return -1
        }
        
        let bind_server = withUnsafeMutablePointer(&server_addr) {
            bind(sock, UnsafeMutablePointer($0), server_addr_size)
        }
        if (bind_server == -1) {
            perror("bind()")
            return -1
        }
        
        let sock_name = withUnsafeMutablePointers(&server_addr, &server_addr_size) {
            getsockname(sock, UnsafeMutablePointer($0), UnsafeMutablePointer($1))
        }
        if (sock_name != 0) {
            perror("getsockname()")
            return -1
        }
        
        let result = server_addr.sin_port.bigEndian
        close(sock)
        
        return Int32(result)
    }
}
