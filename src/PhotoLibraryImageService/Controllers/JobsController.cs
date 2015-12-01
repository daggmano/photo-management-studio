using PhotoLibraryImageService.Jobs;
using PhotoLibraryImageService.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PhotoLibraryImageService.Controllers
{
	public class JobsController : ApiController
	{
		public HttpResponseMessage Get(Guid id)
		{
			JobStates state;
			int progress;

			if (JobsService.GetInstance().GetJobStatus(id, out state, out progress))
			{
				if (state == JobStates.Tombstoned)
				{
					return Request.CreateErrorResponse(HttpStatusCode.Gone, "Job no longer available");
				}

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					jobId = id,
					status = state.ToString().ToLower(),
					progress = progress
				});
			}

			return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No such job");
		}
	}
}
