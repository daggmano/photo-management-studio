using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using MyCouch;
using MyCouch.Requests;
using PhotoLibraryImageService.Data.Interfaces;
using Shared;
using System;

namespace PhotoLibraryImageService.Data
{
    public class DataService : IDataService
    {
        private readonly string _couchDbRoot;
		private readonly string _couchDbName;

        public DataService()
        {
			var dbPath = ConfigurationManager.AppSettings["CouchDbPath"];
			var uri = new Uri(dbPath);
			_couchDbName = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
			_couchDbRoot = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
        }

        public async Task<ServerDatabaseIdentifierObject> GetServerDatabaseIdentifier()
        {
            using (var store = new MyCouchStore(_couchDbRoot, _couchDbName))
            {
                var serverIdQuery = new QueryViewRequest("serverId", "get");
                var serverIdRows = await store.Client.Views.QueryAsync<ServerDatabaseIdentifierObject>(serverIdQuery);
                var serverId = serverIdRows.Rows.Select(x => x.Value).FirstOrDefault();

                return serverId;
            }
        }
    }
}
