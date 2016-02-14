using Microsoft.Extensions.Configuration;

namespace Shared
{
	public class AppSettings
	{
		public string CouchDbPath { get; set; }
		public string LibraryPath { get; set; }
		public int UdpListenPort { get; set; }
		public int PhotoServerPort { get; set; }
		public RollbarSettings Rollbar { get; set; }
	}

	public class RollbarSettings
	{
		public string AccessToken { get; set; }
		public string Environment { get; set; }
	}

	public static class SharedConfiguration
	{
		public static AppSettings GetAppSettings()
		{
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appSettings.json");
			var Configuration = builder.Build();

			var appSettings = ConfigurationBinder.Get<AppSettings>(Configuration.GetSection("AppSettings"));

			return appSettings;
		}
	}
}
