using PhotoLibraryImageService.Jobs;
using PhotoLibraryImageService.Services;
using Shared;
using System;
using System.Linq;
using System.Net;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;

namespace PhotoLibraryImageService.Controllers
{
	public class ImportListController : Controller
	{
		private readonly IOptions<AppSettings> _appSettings;

		public ImportListController(IOptions<AppSettings> appSettings)
		{
			_appSettings = appSettings;
		}

		[HttpGet]
		[Route("api/importlist")]
		public IActionResult Get()
		{
			var id = JobsService.GetInstance().SubmitJob(JobTypes.ImportableList, _appSettings);

			if (!id.HasValue)
			{
				return new ObjectResult("Unable to submit job") { StatusCode = (int)HttpStatusCode.InternalServerError };
			}

			return new ObjectResult(new { jobId = id.Value, status = "submitted" });
		}

		[HttpGet]
		[Route("api/importlist/{id}")]
		public IActionResult GetResult(Guid id)
		{
			JobStates state;
			int progress;

			if (!JobsService.GetInstance().GetJobStatus(id, out state, out progress))
			{
				return new ObjectResult("No such job") { StatusCode = (int)HttpStatusCode.NotFound };
			}

			switch (state)
			{
				case JobStates.Tombstoned:
					return new ObjectResult("Job no longer available") { StatusCode = (int)HttpStatusCode.Gone };
				case JobStates.Error:
					return new ObjectResult("Job is in error state") { StatusCode = (int)HttpStatusCode.PreconditionFailed };
				case JobStates.Running:
					return new ObjectResult("Job is still running") { StatusCode = (int)HttpStatusCode.PreconditionFailed };
				case JobStates.Submitted:
					return new ObjectResult("Job has not started yet") { StatusCode = (int)HttpStatusCode.PreconditionFailed };
				case JobStates.Unknown:
					return new ObjectResult("Job is in an unknown state") { StatusCode = (int)HttpStatusCode.PreconditionFailed };
				default:
					break;
			}

			var jobResult = JobsService.GetInstance().GetResult<ImportableListJobResult>(id);

			if (jobResult != null)
			{
				var urlBase = $"{Request.Scheme}://{Request.Host}";
				var result = new ImportableListObject
				{
					ItemCount = jobResult.ImportableFiles.Count,
					ImportablePhotos = jobResult.ImportableFiles.Select(x => new ImportableItem
					{
						FullPath = x,
						Filename = x.Split('/').Last(),
						ThumbUrl = $"{urlBase}/api/image?path={Uri.EscapeDataString(x)}"
					}).ToList()
				};

				return new ObjectResult(result);
			}

			return new ObjectResult("Job complete without error, but no result available") { StatusCode = (int)HttpStatusCode.InternalServerError };
		}
	}
}
