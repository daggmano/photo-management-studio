using System.Configuration;

namespace PhotoLibraryImageService.Services
{
    public static class SettingsService
    {
        public static string GetLibraryPath()
        {
            return ConfigurationManager.AppSettings["LibraryPath"];
        }
    }
}
