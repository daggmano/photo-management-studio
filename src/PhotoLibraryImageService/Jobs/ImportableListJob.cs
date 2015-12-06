using System;
using Shared;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileManager;

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

		private FileManagementService _fileManagementService;

		private ImportableListJobResult _result;

		BackgroundWorker _worker;

		public ImportableListJob(Guid id) : base(id)
		{
			_progress = 0;
			_result = null;

			_fileManagementService = new FileManagementService();

			var dbPath = ConfigurationManager.AppSettings["CouchDbPath"];
			var uri = new Uri(dbPath);
			_couchDbName = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
			_couchDbRoot = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
		}

		public override void Run()
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
			var worker = sender as BackgroundWorker;

			// Need to get list of media from database
			var dbMediaTask = _fileManagementService.GetAllPhotoPaths();
			Task.WaitAll(dbMediaTask);
			var dbMediaList = dbMediaTask.Result;
	
			var dbMediaFiles = dbMediaList.Select(x => new Tuple<string, string>(x.FullFilePath.ToLowerInvariant(), x.FullFilePath)).ToList();
			var dbLoweredMediaFiles = dbMediaFiles.Select(x => x.Item1).ToList();
			worker.ReportProgress(33);

			// Get list of files from file system
			var rootPath = ConfigurationManager.AppSettings["LibraryPath"];
			if (!rootPath.EndsWith("\\"))
			{
				rootPath += "\\";
			}
			var diskFileList = _fileManagementService.GetFileList(rootPath);

			var diskFiles = diskFileList.Select(x => new Tuple<string, string>(x.ToLowerInvariant(), x)).ToList();
			var loweredDiskFiles = diskFiles.Select(x => x.Item1).ToList();

			worker.ReportProgress(66);

			// Compare...
			var missingFiles = dbMediaFiles.Where(x => !loweredDiskFiles.Contains(x.Item1)).Select(x => x.Item2.Replace("\\", "/")).ToList();
			var unimportedFiles = diskFiles.Where(x => !dbLoweredMediaFiles.Contains(x.Item1)).Select(x => x.Item2.Replace("\\", "/")).ToList();

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
