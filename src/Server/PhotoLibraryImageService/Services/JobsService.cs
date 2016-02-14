using PhotoLibraryImageService.Jobs;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoLibraryImageService.Services
{
	public class JobsService
	{
		private readonly TimeSpan _expiryDuration = TimeSpan.FromMinutes(10);

		private static JobsService _instance = new JobsService();
		private Dictionary<Guid, Job> _jobList;
		private Dictionary<Guid, DateTime> _jobExpiredList;

		public static JobsService GetInstance()
		{
			return _instance;
		}

		private JobsService()
		{
			_jobList = new Dictionary<Guid, Job>();
			_jobExpiredList = new Dictionary<Guid, DateTime>();
		}

		public Guid? SubmitJob(JobTypes jobType)
		{
			return SubmitJob(jobType, "");
		}

		public Guid? SubmitJob<T>(JobTypes jobType, T arg) where T : class
		{
			Job job = null;
			var newJobId = Guid.NewGuid();

			switch (jobType)
			{
				case JobTypes.ImportableList:
					job = new ImportableListJob(newJobId);
					break;

				case JobTypes.ImportPhotos:
					if (typeof(IEnumerable<string>).IsAssignableFrom(typeof(T)))
					{
						job = new ImportJob(newJobId, arg as IEnumerable<string>);
					}
					break;

				default:
					break;
			}

			if (job != null)
			{
				_jobList.Add(newJobId, job);
				job.Run();

				return newJobId;
			}

			return null;
		}

		public bool GetJobStatus(Guid jobId, out JobStates state, out int progress)
		{
			state = JobStates.Unknown;
			progress = -1;

			if (_jobList.ContainsKey(jobId))
			{
				var job = _jobList[jobId];
				job.GetJobStatus(out state, out progress);

				return true;
			}

			if (_jobExpiredList.ContainsKey(jobId) && _jobExpiredList[jobId] >= DateTime.Now)
			{
				state = JobStates.Tombstoned;
				return true;
			}

			return false;
		}

		public ErrorMessageReponse GetJobError(Guid jobId)
		{
			// TODO: Handle errors better :)

			ErrorMessageReponse result = null;

			JobStates state;
			int progress;

			if (_jobList.ContainsKey(jobId))
			{
				var job = _jobList[jobId];
				job.GetJobStatus(out state, out progress);

				if (state == JobStates.Error)
				{
					result = job.GetJobError();
				}
			}

			return result;
		}

		public T GetResult<T>(Guid jobId) where T : class
		{
			var result = default(T);

			JobStates state;
			int progress;

			if (_jobList.ContainsKey(jobId))
			{
				var job = _jobList[jobId];
				job.GetJobStatus(out state, out progress);

				if (state == JobStates.Complete)
				{
					result = job.GetResult<T>();

					ExpireJob(jobId);
				}
			}

			return result;
		}

		private void ExpireJob(Guid jobId)
		{
			if (_jobList.ContainsKey(jobId))
			{
				var job = _jobList[jobId];
				job.Cleanup();

				_jobList.Remove(jobId);

				_jobExpiredList.Add(jobId, DateTime.Now.Add(_expiryDuration));

				var tombstoned = _jobExpiredList.Where(x => x.Value < DateTime.Now).Select(x => x.Key).ToList();
				foreach (var t in tombstoned)
				{
					_jobExpiredList.Remove(t);
				}
			}
		}
	}
}
