import Foundation

// =====================================================

enum LogLevel: Int {
    case Debug = 1, Info, Error
}

let log_level = LogLevel.Debug

protocol Loggable {
    func log()
    func log_error()
    func log_debug()
}

extension String: Loggable {
    func log() {
        if log_level.rawValue <= LogLevel.Info.rawValue {
            print("[info]\t\(self)")
        }
    }
    
    func log_error() {
        if log_level.rawValue <= LogLevel.Error.rawValue {
            print("[error]\t\(self)")
        }
    }
    
    func log_debug() {
        if log_level.rawValue <= LogLevel.Debug.rawValue {
            print("[debug]\t\(self)")
        }
    }
}

// =====================================================

typealias Callback = (AnyObject) -> Void

struct Event {
    static var events = Dictionary<String, Array<Callback>>()
    
    static func register(event: String, callback: Callback) {
        if (self.events[event] == nil) {
            "Initializing list for event '\(event)'".log_debug()
            self.events[event] = Array<Callback>()
        }
        
        if var callbacks = self.events[event] {
            callbacks.append(callback)
            self.events[event] = callbacks
            "Registered callback for event '\(event)'".log_debug()
        } else {
            "Failed to register callback for event \(event)".log_error()
        }
    }
    
    static func emit(event: String, obj: AnyObject) {
        "Emitting event '\(event)'".log_debug()
        if let events = self.events[event] {
            "Found list for event '\(event)', of length \(events.count)".log_debug()
            for callback in events {
                callback(obj)
            }
        } else {
            "Could not find callbacks for event '\(event)'".log_error()
        }
    }
}

// =====================================================

//protocol Evented {
//    func emit(message: String) -> Bool
//}
