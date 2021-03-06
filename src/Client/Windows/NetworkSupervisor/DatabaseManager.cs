﻿using System;
using System.Diagnostics;
using System.Net;
using MyCouch;
using MyCouch.Requests;
using ErrorReporting;

namespace NetworkSupervisor
{
    public class DatabaseManager
    {
        public static async void SetupReplication(IPAddress remoteAddress, string serverId)
        {
            var remote = String.Format("http://{0}:5984/photos", remoteAddress);
			var localName = String.Format("photos_{0}", serverId);
            var local = String.Format("http://localhost:5984/{0}", localName);

			// Need to test if local DB exists, and create if not
			
            var remoteToLocalName = String.Format("repPhotosR2L_{0}", serverId);
            var localToRemoteName = String.Format("repPhotosL2R_{0}", serverId);

            using (var client = new MyCouchServerClient("http://localhost:5984"))
            {
				var databases = client.Databases;
				var local_db = await databases.GetAsync(localName);
				if (local_db.Error != null)
				{
					await databases.PutAsync(localName);
				}

                var request1 = new ReplicateDatabaseRequest(remoteToLocalName, remote, local)
                {
                    Continuous = true,
                    CreateTarget = true
                };

                var request2 = new ReplicateDatabaseRequest(localToRemoteName, local, remote)
                {
                    Continuous = true,
                    CreateTarget = false
                };

                try
                {
                    await client.Replicator.ReplicateAsync(request1);
                    await client.Replicator.ReplicateAsync(request2);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception: " + ex.Message);
					ErrorReporter.SendException(ex);
                }
            }
            
        }
    }
}
