using System.Net.Http;

namespace PhotoLibraryImageService.Helpers
{
	public static class HttpRequestExtensions
	{
		public static string GetSelfLink(this HttpRequestMessage request)
		{
			var scheme = request.RequestUri.Scheme;
			var authority = request.RequestUri.Authority;
			var path = request.RequestUri.AbsolutePath;

			return $"{scheme}://{authority}{path}";
		}
	}
}
