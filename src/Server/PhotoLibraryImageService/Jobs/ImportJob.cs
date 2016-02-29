using System;
using Shared;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using FileManager;
using System.Diagnostics;
using PhotoLibraryImageService.Data.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;

namespace PhotoLibraryImageService.Jobs
{
	public class ImportJob : Job
	{
		private readonly IOptions<AppSettings> _appSettings;

		private readonly object _lock;
		private int _progress;

		private readonly string _couchDbName;
		private readonly string _couchDbRoot;
		private readonly Guid _importTagId;

		private readonly List<string> _args;

		private FileProcessor _fileProcessor;

		[FromServices]
		public IDataService _dataService { get; set; }


//		private ImportableListJobResult _result;

		BackgroundWorker _worker;

		public ImportJob(Guid id, IEnumerable<string> args, IOptions<AppSettings> appSettings) : base(id)
		{
			_args = args.ToList();
			_lock = new object();
			_appSettings = appSettings;

			_progress = 0;
//			_result = null;

			var dbPath = _appSettings.Value.CouchDbPath;
			var uri = new Uri(dbPath);
			_couchDbName = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
			_couchDbRoot = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);

			_importTagId = id;

			_fileProcessor = new FileProcessor();
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
			var rootPath = _appSettings.Value.LibraryPath;
			if (!rootPath.EndsWith("\\"))
			{
				rootPath += "\\";
			}
			var progressStep = (int)Math.Ceiling(100.0 / _args.Count);
			var progress = 0;

			// Need to create import tag first...
			var importTagTask = _dataService.CreateImportTag(_importTagId, DateTime.UtcNow);
			Task.WaitAll(importTagTask);
			var importTag = importTagTask.Result;

			foreach (var path in _args)
			{
				var fullPath = path.Replace("/", "\\");

				var media = _fileProcessor.ProcessFile(fullPath, rootPath, new Guid(importTag.ImportId));

				// TODO: Check if file is already in db before importing (check loweredFileName).  May need to lock this to prevent clashing.
				lock (_lock)
				{
					var mediaExistsTask = _dataService.MediaExists(media.MediaId);
					Task.WaitAll(mediaExistsTask);
					if (mediaExistsTask.Result)
					{
						Debug.WriteLine($"Media with path '{media.MediaId}' already in database, skipping.");
					}
					else
					{
						var insertMediaTask = _dataService.InsertMedia(media);
						Task.WaitAll(insertMediaTask);
					}
				}

				progress = Math.Min(progress + progressStep, 100);
				worker.ReportProgress(progress);
			}
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
			throw new NotImplementedException();
		}

		public override void Cleanup()
		{
			throw new NotImplementedException();
		}
	}
}
