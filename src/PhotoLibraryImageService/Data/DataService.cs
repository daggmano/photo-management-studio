using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using MyCouch;
using MyCouch.Requests;
using PhotoLibraryImageService.Data.Interfaces;
using Shared;

namespace PhotoLibraryImageService.Data
{
    public class DataService : IDataService
    {
        private readonly string _couchDbUrl;

        public DataService()
        {
            _couchDbUrl = ConfigurationManager.AppSettings["CouchDbPath"];
        }

        public async Task<ServerDatabaseIdentifierObject> GetServerDatabaseIdentifier()
        {
            using (var store = new MyCouchStore(_couchDbUrl))
            {
                var serverIdQuery = new QueryViewRequest("serverId", "get");
                var serverIdRows = await store.Client.Views.QueryAsync<ServerDatabaseIdentifierObject>(serverIdQuery);
                var serverId = serverIdRows.Rows.Select(x => x.Value).FirstOrDefault();

                return serverId;
            }
        }
    }
}
