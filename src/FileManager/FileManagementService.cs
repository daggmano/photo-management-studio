using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileManager.Models;
using MyCouch;
using MyCouch.Requests;
using ErrorReporting;

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

        public async Task<IEnumerable<MediaSimple>> GetAllPhotoPaths()
        {
            using (var store = new MyCouchStore(_couchDbRoot, _couchDbName))
            {
                var mediaQuery = new QueryViewRequest("media", "all");
                var mediaRows = await store.Client.Views.QueryAsync<MediaSimple>(mediaQuery);
                var media = mediaRows.Rows.Select(x => x.Value).ToList();

                return media;
            }
        }
    }
}
