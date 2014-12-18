using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileManager.Models;
using MyCouch;
using MyCouch.Requests;

namespace FileManager
{
    public class FileManagementService
    {
        private readonly string _couchDbUrl;

        public FileManagementService()
        {
            _couchDbUrl = ConfigurationManager.AppSettings["CouchDbPath"];
        }

        public IEnumerable<string> GetFileList(string rootFolder)
        {
            var result = new List<string>();

            try
            {
                var files = Directory.GetFiles(rootFolder, "*.*", SearchOption.AllDirectories);
                result.AddRange(files.Select(x => x.Replace(rootFolder, "")));
            }
            catch (UnauthorizedAccessException)
            {}

            return result;
        }

        public async Task<IEnumerable<MediaSimple>> GetAllPhotoPaths()
        {
            using (var store = new MyCouchStore(_couchDbUrl + "photos"))
            {
                var mediaQuery = new QueryViewRequest("media", "all");
                var mediaRows = await store.Client.Views.QueryAsync<MediaSimple>(mediaQuery);
                var media = mediaRows.Rows.Select(x => x.Value).ToList();

                return media;
            }
        }
    }
}
