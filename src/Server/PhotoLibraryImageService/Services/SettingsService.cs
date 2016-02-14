using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Shared;

namespace PhotoLibraryImageService.Services
{
	public class SettingsService
	{
		private static SettingsService _instance = null;
		
		public static SettingsService Instance {
			get {
				if (_instance == null) {
					_instance = new SettingsService();
				}
				
				return _instance;
			}
		}
		
		[FromServices]
		public IOptions<AppSettings> _appSettings { get; set; }
		
		public string GetLibraryPath()
		{
			return _appSettings.Value.LibraryPath;
		}
	}
}
