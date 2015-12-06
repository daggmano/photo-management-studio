using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyCouch;
using ErrorReporting;
using DataTypes;

namespace FileManager
{
    public class FileManagementService
    {
		private readonly string _couchDbName;
		private readonly string _couchDbRoot;

        public FileManagementService()
        {
			var dbPath = ConfigurationManager.AppSettings["CouchDbPath"];
			var uri = new Uri(dbPath);
			_couchDbName = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
			_couchDbRoot = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
		}

		public IEnumerable<string> GetFileList(string rootFolder)
        {
            var result = new List<string>();

            try
            {
                var files = Directory.GetFiles(rootFolder, "*.*", SearchOption.AllDirectories);
                result.AddRange(files.Select(x => x.Replace(rootFolder, "")));
			}
			catch (UnauthorizedAccessException ex)
            {
				ErrorReporter.SendException(ex);
			}

            return result;
        }

        public async Task<IEnumerable<Media>> GetAllPhotoPaths()
        {
            using (var store = new MyCouchStore(_couchDbRoot, _couchDbName))
            {
				var mediaQuery = new Query("media", "all");

				var mediaRows = await store.QueryAsync<Media>(mediaQuery);
                var media = mediaRows.Select(x => x.Value).ToList();

                return media;
            }
        }
    }
}
