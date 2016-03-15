using System;
using Shared;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using FileManager;
using System.Diagnostics;
using PhotoLibraryImageService.Data;
using PhotoLibraryImageService.Data.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;

namespace PhotoLibraryImageService.Jobs
{
	public class ImportJobResult
	{
		public List<string> ImportedFiles { get; set; }
	}

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

		private IDataService _dataService;

		private ImportJobResult _result;

		BackgroundWorker _worker;

		public ImportJob(Guid id, IEnumerable<string> args, IOptions<AppSettings> appSettings) : base(id)
		{
			_args = args.ToList();
			_lock = new object();
			_appSettings = appSettings;

			_dataService = new DataService(appSettings);

			_progress = 0;
			_result = null;

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
			Console.WriteLine("ImportJob Worker");
			Console.WriteLine("================");

			var worker = sender as BackgroundWorker;
			var rootPath = _appSettings.Value.LibraryPath;
			if (!rootPath.EndsWith("/"))
			{
				rootPath += "/";
			}
			Console.WriteLine($"rootPath: {rootPath}");

			var progressStep = (int)Math.Ceiling(100.0 / _args.Count);
			var progress = 0;
			var importedFiles = new List<string>();

			// Need to create import tag first...
			var importTagTask = _dataService.CreateImportTag(_importTagId, DateTime.UtcNow);
			Task.WaitAll(importTagTask);
			var importTag = importTagTask.Result;

			foreach (var path in _args)
			{
				try {
					var media = _fileProcessor.ProcessFile(path, rootPath, new Guid(importTag.ImportId));

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
							importedFiles.Add(path);
						}
					}

					progress = Math.Min(progress + progressStep, 100);
					worker.ReportProgress(progress);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Caught Exception: {ex.Message}");
				}
			}

			_result = new ImportJobResult { ImportedFiles = importedFiles };
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
