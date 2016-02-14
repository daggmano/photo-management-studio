using PhotoLibraryImageService.Jobs;
using PhotoLibraryImageService.Services;
using System;
using System.Net;
using Microsoft.AspNet.Mvc;

namespace PhotoLibraryImageService.Controllers
{
	[Route("api/[controller]")]
	public class JobsController : Controller
	{
		[HttpGet]
		public IActionResult Get(Guid id)
		{
			JobStates state;
			int progress;

			if (JobsService.GetInstance().GetJobStatus(id, out state, out progress))
			{
				if (state == JobStates.Tombstoned)
				{
					return new ObjectResult("Job no longer available") { StatusCode = (int)HttpStatusCode.Gone };
				}

				return new ObjectResult(new
				{
					jobId = id,
					status = state.ToString().ToLower(),
					progress = progress
				});
			}

			return new ObjectResult("No such job") { StatusCode = (int)HttpStatusCode.NotFound };
		}
	}
}
