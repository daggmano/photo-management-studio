using System;
using Shared;

namespace PhotoLibraryImageService.Jobs
{
	public abstract class Job
	{
		public Guid Id { get; }

		protected JobStates _state;

		public Job(Guid id)
		{
			Id = id;
		}

		public abstract void Run();

		public abstract void GetJobStatus(out JobStates state, out int progress);
		public abstract ErrorMessageReponse GetJobError();
		public abstract T GetResult<T>() where T : class;

		public abstract void Cleanup();
	}
}
