import Foundation

/**
 * HTTP
 */
public class HTTP {
    
    public typealias Response = (Result) -> Void
    public typealias Headers = [String: String]
    
    public enum Method: String {
        case GET    = "GET"
        case POST   = "POST"
        case PUT    = "PUT"
        case HEAD   = "HEAD"
        case DELETE = "DELETE"
    }
    
    public enum Result {
        case Success(AnyObject, NSHTTPURLResponse)
        case Error(NSError)
    }
    
    private var request: NSMutableURLRequest
    
    /**
     * Init
     */
    public init(method: Method, url: String) {
        self.request = NSMutableURLRequest(URL: NSURL(string: url)!)
        self.request.HTTPMethod = method.rawValue
    }
    
    public init(method: Method, url: String, headers: Headers?) {
        self.request = NSMutableURLRequest(URL: NSURL(string: url)!)
        self.request.HTTPMethod = method.rawValue
        if let headers = headers {
            self.request.allHTTPHeaderFields = headers
        }
    }
    
    /**
     * Class funcs
     */
     
     // GET
    public class func get(url: String) -> HTTP {
        return HTTP(method: .GET, url: url)
    }
    
    public class func get(url: String, headers: Headers?) -> HTTP {
        return HTTP(method: .GET, url: url, headers: headers)
    }
    
    public class func get(url: String, done: Response) -> HTTP {
        return HTTP.get(url).end(done)
    }
    
    public class func get(url: String, headers: Headers?, done: Response) -> HTTP {
        return HTTP(method: .GET, url: url, headers: headers).end(done)
    }
    
    // POST
    public class func post(url: String) -> HTTP {
        return HTTP(method: .POST, url: url)
    }
    
    public class func post(url: String, headers: Headers?) -> HTTP {
        print("Posting to \(url)")
        return HTTP(method: .POST, url: url, headers: headers)
    }
    
    public class func post(url: String, done: Response) -> HTTP {
        return HTTP.post(url).end(done)
    }
    
    public class func post(url: String, data: AnyObject, done: Response) -> HTTP {
        return HTTP.post(url).send(data).end(done)
    }
    
    public class func post(url: String, headers: Headers?, data: AnyObject, done: Response) -> HTTP {
        return HTTP.post(url, headers: headers).send(data).end(done)
    }
    
    // PUT
    public class func put(url: String) -> HTTP {
        return HTTP(method: .PUT, url: url)
    }
    
    public class func put(url: String, done: Response) -> HTTP {
        return HTTP.put(url).end(done)
    }
    
    public class func put(url: String, data: AnyObject, done: Response) -> HTTP {
        return HTTP.put(url).send(data).end(done)
    }
    
    // DELETE
    public class func delete(url: String) -> HTTP {
        return HTTP(method: .DELETE, url: url)
    }
    
    public class func delete(url: String, done: Response) -> HTTP {
        return HTTP.delete(url).end(done)
    }

    /**
     * Methods
     */
    public func send(data: AnyObject) -> HTTP {
        do {
            var d = try NSJSONSerialization.dataWithJSONObject(data, options: [])
            
            //NSJSONSerialization converts a URL string from http://... to http:\/\/... remove the extra escapes
            var dataStr = NSString.init(data: d, encoding: NSUTF8StringEncoding)
            dataStr = dataStr?.stringByReplacingOccurrencesOfString("\\/", withString: "/")
            d = (dataStr?.dataUsingEncoding(NSUTF8StringEncoding))!

            self.request.HTTPBody = d//try NSJSONSerialization.dataWithJSONObject(data, options: [])
            
            let dataString = NSString(data: self.request.HTTPBody!, encoding: NSUTF8StringEncoding)!
            print("Sending data: \(dataString)")
        } catch {
            self.request.HTTPBody = nil
        }
        self.request.addValue("application/json", forHTTPHeaderField: "Accept")
        self.request.addValue("application/json", forHTTPHeaderField: "Content-Type")
        if let length = self.request.HTTPBody?.length {
            self.request.addValue(String(length), forHTTPHeaderField: "Content-Length")
        } else {
            self.request.addValue("0", forHTTPHeaderField: "Content-Length")
        }
        return self
    }
    
    public func end(done: Response) -> HTTP {
        let session = NSURLSession.sharedSession()
        let task = session.dataTaskWithRequest(self.request) { data, response, error in
            
            // we have an error -> maybe connection lost
            if let error = error {
                done(Result.Error(error))
                return
            }
            
            // request was success
            var json: AnyObject!
            if let data = data {
                do {
                    json = try NSJSONSerialization.JSONObjectWithData(data, options: [])
                } catch let error as NSError {
                    done(Result.Error(error))
                    return
                }
            }
            
            // looking good
            let res = response as! NSHTTPURLResponse
            done(Result.Success(json, res))
        }
        task.resume()
        return self
    }
}
