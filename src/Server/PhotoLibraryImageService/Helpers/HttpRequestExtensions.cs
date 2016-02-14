using Microsoft.AspNet.Http;

namespace PhotoLibraryImageService.Helpers
{
	public static class HttpRequestExtensions
	{
		public static string GetSelfLink(this HttpRequest request)
		{
			var scheme = request.Scheme;
			var authority = request.Host;
			var path = request.Path;
			
//			System.Console.WriteLine("Host: " + request.Host);
//			System.Console.WriteLine("Method: " + request.Method);
//			System.Console.WriteLine("Path: " + request.Path);
//			System.Console.WriteLine("PathBase: " + request.PathBase);
//			System.Console.WriteLine("Protocol: " + request.Protocol);
//			System.Console.WriteLine("Query: " + request.Query);
//			System.Console.WriteLine("QueryString: " + request.QueryString);
//			System.Console.WriteLine("Scheme: " + request.Scheme);

			return $"{scheme}://{authority}{path}";
		}
	}
}
