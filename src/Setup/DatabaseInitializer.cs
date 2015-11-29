using MyCouch;
using MyCouch.Requests;
using Newtonsoft.Json;
using Shared;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Setup
{
	public class DatabaseInitializer
	{
		public async Task Run()
		{
			string docString;

			// Check for database existence
			var dbPath = ConfigurationManager.AppSettings["CouchDbPath"];
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
				Console.WriteLine("Checking whether view for Server Details exists...");
				var serverViewExists = await store.ExistsAsync("_design/serverId");
				if (!serverViewExists)
				{
					Console.WriteLine("Server Details view not found, we'll create it now...");

					docString = JsonConvert.SerializeObject(new
					{
						language = "javascript",
						views = new
						{
							get = new
							{
								map = "function(doc) { if (doc.docType == 'serverId') { emit(null, doc); } }"
							}
						}
					});

					await store.Client.Documents.PutAsync("_design/serverId", docString);
					Console.WriteLine("Server Details view created...");
				}
				else
				{
					Console.WriteLine("Server Details view exists, continuing...");
				}

				Console.WriteLine();

				var dbServerIdQuery = new QueryViewRequest("serverId", "get");
				var dbServerIdRows = await store.Client.Views.QueryAsync<ServerDatabaseIdentifierObject>(dbServerIdQuery);
				var id = dbServerIdRows.Rows.FirstOrDefault()?.Id;
				var rev = string.Empty;

				if (!string.IsNullOrWhiteSpace(id))
				{
					var header = await store.GetHeaderAsync(id);
					rev = header.Rev;
				}

				var dbServerId = dbServerIdRows.Rows.Select(x => x.Value).FirstOrDefault();

				var serverId = dbServerId?.ServerId ?? string.Empty;
				var serverName = dbServerId?.ServerName ?? string.Empty;

				while (true)
				{
					if (GetServerName(ref serverName, ref serverId))
					{
						break;
					}
				}

				docString = JsonConvert.SerializeObject(new
				{
					serverId = serverId,
					docType = "serverId",
					serverName = serverName
				});

				if (string.IsNullOrWhiteSpace(id))
				{
					// create new entry
					Console.WriteLine("Creating new Server Identification entry...");

					await store.Client.Documents.PostAsync(docString);
					Console.WriteLine("Server Identification entry created...");
				}
				else
				{
					Console.WriteLine("Updating existing Server Identification entry...");

					var retval = await store.Client.Documents.PutAsync(id, rev, docString);
					Console.WriteLine("Server Identification entry updated...");
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
								map = "function(doc) { if (doc.docType == 'collection') { emit(null, doc); } }"
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
								map = "function(doc) { if (doc.docType == 'import') { emit(null, doc); } }"
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
								map = "function(doc) { if (doc.docType == 'media') { emit(null, doc); } }"
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
								map = "function(doc) { if (doc.docType == 'tag' && doc.subType == 'parent') { emit(null, doc); } }"
							},
                            buckets = new
							{
								map = "function(doc) { if (doc.docType == 'tag' && doc.subType == 'bucket') { emit(null, doc); } }"
							},
                            tags = new
							{
								map = "function(doc) { if (doc.docType == 'tag' && doc.subType == 'tag') { emit(null, doc); } }"
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
