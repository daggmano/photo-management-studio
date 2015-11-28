using MyCouch;
using MyCouch.Requests;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setup
{
	public class DatabaseInitializer
	{
		public async Task Run()
		{
			// Check for database existence
			var dbPath = ConfigurationManager.AppSettings["CouchDbPath"];
			var uri = new Uri(dbPath);
			var dbName = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
			var dbRoot = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);

			Console.WriteLine($"Connecting to CouchDB database at {dbRoot}...");

			using (var client = new MyCouchServerClient(dbRoot))
			{
				// Creates the database, but only if it does not exist.
				Console.WriteLine($"Looking for database '{dbName}'...");

				var db = await client.Databases.GetAsync(dbName);
				if (!string.IsNullOrWhiteSpace(db.Error) && db.Error.Equals("not_found"))
				{
					Console.WriteLine($"Database '{dbName}' not found, we'll create it now...");

					await client.Databases.PutAsync(dbName);
					Console.WriteLine($"Database '{dbName}' created...");
				} else
				{
					Console.WriteLine($"Database '{dbName}' exists, continuing...");
                }
			}

			Console.WriteLine();

			using (var store = new MyCouchStore(dbRoot, dbName))
			{
				Console.WriteLine("Checking whether view for Server Details exists...");
				var serverViewExists = await store.ExistsAsync("_design/serverId");
				if (!serverViewExists)
				{
					Console.WriteLine("Server Details view not found, we'll create it now...");

					var doc = new
					{
						language = "javascript",
						views = new
						{
							get = new
							{
								map = "function(doc) { if (doc.type == 'server' && doc.subtype == 'server_id') { emit(doc._id, doc); } }"
							}
						}
					};
					var docString = JsonConvert.SerializeObject(doc);

					await store.Client.Documents.PutAsync("_design/serverId", docString);
				}
				else
				{
					Console.WriteLine("Server Details view exists, continuing...");
				}


				var serverIdQuery = new QueryViewRequest("serverId", "get");
				var serverIdRows = await store.Client.Views.QueryAsync<ServerDatabaseIdentifierObject>(serverIdQuery);

				// TODO: Need to check above to get existing name and id.

				string serverName, serverId;
				while (true)
				{
					if (GetServerName(out serverName, out serverId))
					{
						break;
					}
				}

				
			}

			// Check for Server ID entries, get user prefs if required and create entries

			// Check for default DB entries - not sure what they are yet
		}

		private bool GetServerName(out string serverName, out string serverId)
		{
			serverName = null;

			while (string.IsNullOrWhiteSpace(serverName))
			{
				Console.Write("Enter a name for this server (e.g. My Photo Server): ");
				serverName = Console.ReadLine();
			}

			serverId = string.Join("-", serverName.Split(' ')).ToLowerInvariant();

			Console.WriteLine();
			Console.WriteLine("We'll use the following to identify the server:");
			Console.WriteLine($"\tServer Name: {serverName}");
			Console.WriteLine($"\tServer ID: {serverId}");
			Console.Write("Is this correct? [Y/n] ");

			var response = Console.ReadLine();
			if (response.Length > 0 && response.ToLower().ElementAt(0) == 'n')
			{
				return false;
			}
			return true;
        }
	}
}
