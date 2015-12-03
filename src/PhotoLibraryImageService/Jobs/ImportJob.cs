using System;
using Shared;
using System.ComponentModel;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using PhotoLibraryImageService.Services;
using FileManager;
using Newtonsoft.Json;
using System.Diagnostics;

namespace PhotoLibraryImageService.Jobs
{
	public class ImportJob : Job
	{
		private int _progress;

		private readonly string _couchDbName;
		private readonly string _couchDbRoot;
		private readonly Guid _importTagId;

		private readonly List<string> _args;

		private FileProcessor _fileProcessor;

//		private ImportableListJobResult _result;

		BackgroundWorker _worker;

		public ImportJob(Guid id, IEnumerable<string> args) : base(id)
		{
			_args = args.ToList();

			_progress = 0;
//			_result = null;

			var dbPath = ConfigurationManager.AppSettings["CouchDbPath"];
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
			var rootPath = ConfigurationManager.AppSettings["LibraryPath"];
			if (!rootPath.EndsWith("\\"))
			{
				rootPath += "\\";
			}
			var progressStep = (int)Math.Ceiling(100.0 / _args.Count);
			var progress = 0;

			foreach (var path in _args)
			{
				var fullPath = path.Replace("/", "\\");

				var media = _fileProcessor.ProcessFile(fullPath, rootPath, _importTagId.ToString());

				var str = JsonConvert.SerializeObject(media);
				Debug.WriteLine(str);

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
