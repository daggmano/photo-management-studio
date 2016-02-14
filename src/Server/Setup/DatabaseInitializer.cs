using DataTypes;
using MyCouch;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Shared;

namespace Setup
{
	public class DatabaseInitializer
	{
		private AppSettings _appSettings;
		
		public DatabaseInitializer()
		{
			_appSettings = SharedConfiguration.GetAppSettings();
		}
		
		public async Task Run()
		{
			string docString;

			// Check for database existence

			var dbPath = _appSettings.CouchDbPath;
			var uri = new Uri(dbPath);
			var dbName = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
			var dbRoot = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);

			Console.WriteLine("NOTE: Before running Photo Management Studio, CouchDB must be installed and");
			Console.WriteLine("      configured to allow all clients to connect to allow replication to");
			Console.WriteLine("      client installations.  Please edit the local.ini file, probably located at");
			Console.WriteLine("      C:\\Program Files(x86)\\Apache Software Foundation\\CouchDB\\etc\\couchdb");
			Console.WriteLine("      and add the following line under the [httpd] section:");
			Console.WriteLine();
			Console.WriteLine("      bind_address = 0.0.0.0");
			Console.WriteLine();
			Console.WriteLine();

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
				Console.WriteLine("Checking whether view for Server Detail exists...");
				var serverViewExists = await store.ExistsAsync("_design/serverDetail");
				if (!serverViewExists)
				{
					Console.WriteLine("Server Detail view not found, we'll create it now...");

					docString = JsonConvert.SerializeObject(new
					{
						language = "javascript",
						views = new
						{
							get = new
							{
								map = "function(doc) { if (doc.$doctype == 'serverDetail') { emit(null, doc); } }"
							}
						}
					});

					await store.Client.Documents.PutAsync("_design/serverDetail", docString);
					Console.WriteLine("Server Detail view created...");
				}
				else
				{
					Console.WriteLine("Server Detail view exists, continuing...");
				}

				Console.WriteLine();

				var dbServerDetailQuery = new Query("serverDetail", "get");
				var dbServerDetailRows = await store.QueryAsync<ServerDetail>(dbServerDetailQuery);

				var dbServerDetail = dbServerDetailRows.Select(x => x.Value).FirstOrDefault();
				if (dbServerDetail == null)
				{
					dbServerDetail = new ServerDetail
					{
						ServerDetailId = Guid.NewGuid().ToString(),
						ServerId = String.Empty,
						ServerName = String.Empty
					};
				}

				var serverId = dbServerDetail.ServerId;
				var serverName = dbServerDetail.ServerName;

				while (true)
				{
					if (GetServerName(ref serverName, ref serverId))
					{
						break;
					}
				}

				if (serverName.Equals(dbServerDetail.ServerName) && serverId.Equals(dbServerDetail.ServerId))
				{
					Console.WriteLine("No change to Server Identification, continuing...");
				}
				else
				{
					dbServerDetail.ServerId = serverId;
					dbServerDetail.ServerName = serverName;

					if (String.IsNullOrWhiteSpace(dbServerDetail.ServerDetailRev))
					{
						// create new entry
						Console.WriteLine("Creating new Server Identification entry...");
						dbServerDetail = await store.StoreAsync(dbServerDetail);
						Console.WriteLine("Server Identification entry created...");
					}
					else
					{
						Console.WriteLine("Updating existing Server Identification entry...");
						dbServerDetail = await store.StoreAsync(dbServerDetail);
						Console.WriteLine("Server Identification entry updated...");
					}
				}

				// Set up Default Views
				Console.WriteLine();

				serverViewExists = await store.ExistsAsync("_design/collections");
				if (!serverViewExists)
				{
					Console.WriteLine("Creating Collections View...");

					docString = JsonConvert.SerializeObject(new
					{
						language = "javascript",
						views = new
						{
							all = new
							{
								map = "function(doc) { if (doc.$doctype == 'collection') { emit(null, doc); } }"
							}
						}
					});

					await store.Client.Documents.PutAsync("_design/collections", docString);
				}

				serverViewExists = await store.ExistsAsync("_design/imports");
				if (!serverViewExists)
				{
					Console.WriteLine("Creating Imports View...");

					docString = JsonConvert.SerializeObject(new
					{
						language = "javascript",
						views = new
						{
							all = new
							{
								map = "function(doc) { if (doc.$doctype == 'import') { emit(null, doc); } }"
							}
						}
					});

					await store.Client.Documents.PutAsync("_design/imports", docString);
				}

				serverViewExists = await store.ExistsAsync("_design/media");
				if (!serverViewExists)
				{
					Console.WriteLine("Creating Media View...");

					docString = JsonConvert.SerializeObject(new
					{
						language = "javascript",
						views = new
						{
							all = new
							{
								map = "function(doc) { if (doc.$doctype == 'media') { emit(null, doc); } }"
							}
						}
					});

					await store.Client.Documents.PutAsync("_design/media", docString);
				}

				serverViewExists = await store.ExistsAsync("_design/tags");
				if (!serverViewExists)
				{
					Console.WriteLine("Creating Tags View...");

					docString = JsonConvert.SerializeObject(new
					{
						language = "javascript",
						views = new
						{
							parents = new
							{
								map = "function(doc) { if (doc.$doctype == 'tag' && doc.subType == 'parent') { emit(null, doc); } }"
							},
                            buckets = new
							{
								map = "function(doc) { if (doc.$doctype == 'tag' && doc.subType == 'bucket') { emit(null, doc); } }"
							},
                            tags = new
							{
								map = "function(doc) { if (doc.$doctype == 'tag' && doc.subType == 'tag') { emit(null, doc); } }"
							},
						}
					});

					await store.Client.Documents.PutAsync("_design/tags", docString);
				}
			}
		}

		private bool GetServerName(ref string serverName, ref string serverId)
		{
			do
			{
				Console.Write("Enter a name for this server (e.g. My Photo Server): ");
				if (!string.IsNullOrWhiteSpace(serverName))
				{
					Console.Write($" [current name: {serverName}] ");
				}
				var name = Console.ReadLine();
				if (!string.IsNullOrWhiteSpace(name))
				{
					serverName = name;
				}
			} while (string.IsNullOrWhiteSpace(serverName));

			serverId = string.IsNullOrWhiteSpace(serverId) ? string.Join("-", serverName.Split(' ')).ToLowerInvariant() : serverId;

			Console.WriteLine();
			Console.WriteLine("We'll use the following to identify the server.  Note: Once the Server ID has been set it cannot be changed.");
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
