using PhotoLibraryImageService.Jobs;
using PhotoLibraryImageService.Services;
using Shared;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PhotoLibraryImageService.Controllers
{
	public class ImportListController : ApiController
	{
		public HttpResponseMessage Get()
		{
			var id = JobsService.GetInstance().SubmitJob(JobTypes.ImportableList);

			if (!id.HasValue)
			{
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Unable to submit job");
			}

			return Request.CreateResponse(HttpStatusCode.OK, new { jobId = id.Value, status = "submitted" });
		}

		public HttpResponseMessage Get(Guid id)
		{
			JobStates state;
			int progress;

			if (!JobsService.GetInstance().GetJobStatus(id, out state, out progress))
			{
				return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No such job");
			}

			switch (state)
			{
				case JobStates.Tombstoned:
					return Request.CreateErrorResponse(HttpStatusCode.Gone, "Job no longer available");
				case JobStates.Error:
					return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Job is in error state");
				case JobStates.Running:
					return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Job is still running");
				case JobStates.Submitted:
					return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Job has not yet started");
				case JobStates.Unknown:
					return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Job is in an unknown state");
				default:
					break;
			}

			var result = JobsService.GetInstance().GetResult<ImportableListObject>(id);

			if (result != null)
			{
				return Request.CreateResponse(HttpStatusCode.OK, result);
			}

			return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Job complete without error, but no result available");
		}
	}
}
