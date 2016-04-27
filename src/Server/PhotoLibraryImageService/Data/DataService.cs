using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using MyCouch;
using PhotoLibraryImageService.Data.Interfaces;
using System;
using DataTypes;
using Shared;

namespace PhotoLibraryImageService.Data
{
	public class DataService : IDataService
	{
		private readonly AppSettings _appSettings;		
		private readonly string _couchDbRoot;
		private readonly string _couchDbName;

		public DataService(IOptions<AppSettings> options)
		{
			_appSettings = options.Value;
			
			var dbPath = _appSettings.CouchDbPath;
			var uri = new Uri(dbPath);
			_couchDbName = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
			_couchDbRoot = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
		}

		public async Task<ServerDetail> GetServerDatabaseIdentifier()
		{
			using (var store = new MyCouchStore(_couchDbRoot, _couchDbName))
			{
				var serverDetailQuery = new Query("serverDetail", "get");
				var serverDetailRows = await store.QueryAsync<ServerDetail>(serverDetailQuery);
				var serverDetail = serverDetailRows.Select(x => x.Value).FirstOrDefault();

				return serverDetail;
			}
		}

		public async Task<Import> CreateImportTag(Guid tagId, DateTime importDate)
		{
			var import = new Import
			{
				ImportId = tagId.ToString(),
				ImportDate = importDate
			};

			using (var store = new MyCouchStore(_couchDbRoot, _couchDbName))
			{
				var docHeader = await store.StoreAsync(import);
				return import;
			}
		}

		public async Task<Media> InsertMedia(Media media)
		{
			using (var store = new MyCouchStore(_couchDbRoot, _couchDbName))
			{
				var docHeader = await store.StoreAsync(media);
				return media;
			}
		}

		public async Task<bool> MediaExists(string loweredFilePath)
		{
			using (var store = new MyCouchStore(_couchDbRoot, _couchDbName))
			{
				return await store.ExistsAsync(loweredFilePath);
			}
		}
		
		public async Task<Media> GetMedia(string uid)
		{
			using (var store = new MyCouchStore(_couchDbRoot, _couchDbName))
			{
				var items = await store.QueryAsync<Media>(new Query("media", "all"));
				Console.WriteLine("Number of items: " + items.Count());
				var media = items.Select(x => x.Value).SingleOrDefault(x => x.UniqueId == uid);
				
				return media;
			}
		}
	}
}
