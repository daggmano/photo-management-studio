using System;
using Shared;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using MyCouch;
using MyCouch.Requests;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PhotoLibraryImageService.Jobs
{
	public class ImportableListJobResult
	{
		public List<string> MissingFiles { get; set; }
		public List<string> ImportableFiles { get; set; }
	}

	public class ImportableListJob : Job
	{
		private int _progress;
		private string _couchDbName;
		private string _couchDbRoot;

		private ImportableListJobResult _result;

		BackgroundWorker _worker;

		public ImportableListJob(Guid id) : base(id)
		{
			_progress = 0;
			_result = null;

			var dbPath = ConfigurationManager.AppSettings["CouchDbPath"];
			var uri = new Uri(dbPath);
			_couchDbName = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
			_couchDbRoot = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
		}

		public override void Run(params object[] args)
		{
			_worker = new BackgroundWorker
			{
				WorkerReportsProgress = true
			};

			_worker.DoWork += Worker_DoWork;

			_worker.ProgressChanged += (sender, e) =>
			{
				_progress = e.ProgressPercentage;
			};

			_worker.RunWorkerCompleted += (sender, e) =>
			{
				if (_state != JobStates.Error)
				{
					_state = JobStates.Complete;
				}
			};

			_worker.RunWorkerAsync();

			_state = JobStates.Running;
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			List<MediaObject> dbMediaList;

			// Need to get list of media from database
			using (var store = new MyCouchStore(_couchDbRoot, _couchDbName))
			{
				var mediaQuery = new QueryViewRequest("media", "all");

				var task = store.Client.Views.QueryAsync<MediaObject>(mediaQuery);
				Task.WaitAll(task);
				var mediaRows = task.Result;
				dbMediaList = mediaRows.Rows.Select(x => x.Value).ToList();
			}

			var dbLoweredMediaFiles = dbMediaList.Select(x => x.FullFilePath.ToLowerInvariant()).ToList();
			worker.ReportProgress(33);

			// Get list of files from file system
			var rootPath = ConfigurationManager.AppSettings["LibraryPath"];
			if (!rootPath.EndsWith("\\"))
			{
				rootPath += "\\";
			}
			// TODO: Need to remove Folders from here, keep only files
			var entries = Directory.GetFileSystemEntries(rootPath, "*", SearchOption.AllDirectories);

			var loweredDiskFiles = new List<string>();
			foreach (var entry in entries)
			{
				FileAttributes attr = File.GetAttributes(entry);

				// Only process files
				if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
				{
					loweredDiskFiles.Add(entry.Substring(rootPath.Length).ToLowerInvariant());
				}
			}

			worker.ReportProgress(66);

			// Compare...
			var missingFiles = dbLoweredMediaFiles.Where(x => !loweredDiskFiles.Contains(x)).Select(x => x.Replace("\\", "/")).ToList();
			var unimportedFiles = loweredDiskFiles.Where(x => !dbLoweredMediaFiles.Contains(x)).Select(x => x.Replace("\\", "/")).ToList();

			_result = new ImportableListJobResult
			{
				MissingFiles = missingFiles,
				ImportableFiles = unimportedFiles
			};

			worker.ReportProgress(100);
		}

		public override void GetJobStatus(out JobStates state, out int progress)
		{
			state = _state;
			progress = _progress;
		}

		public override ErrorMessageReponse GetJobError()
		{
			throw new NotImplementedException();
		}

		public override T GetResult<T>()
		{
			return _result as T;
		}

		public override void Cleanup()
		{
		}
	}
}
