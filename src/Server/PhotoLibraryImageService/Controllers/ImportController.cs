using PhotoLibraryImageService.Jobs;
using PhotoLibraryImageService.Services;
using Shared;
using System;
using System.Net;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;

namespace PhotoLibraryImageService.Controllers
{
	[Route("api/[controller]")]
	public class ImportController : Controller
	{
		private readonly IOptions<AppSettings> _appSettings;

		public ImportController(IOptions<AppSettings> appSettings)
		{
			_appSettings = appSettings;
		}

		[HttpPost]
		public IActionResult Post([FromBody] ImportPhotosRequestObject request)
		{
			var id = JobsService.GetInstance().SubmitJob(JobTypes.ImportPhotos, request.PhotoPaths, _appSettings);

			if (!id.HasValue)
			{
				return new ObjectResult("Unable to submit job") { StatusCode = (int)HttpStatusCode.InternalServerError };
			}

			return new ObjectResult(new { jobId = id.Value, status = "submitted" });
		}

		[HttpGet]
		public IActionResult Get(Guid id)
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

			//var jobResult = JobsService.GetInstance().GetResult<ImportableListJobResult>(id);

			//if (jobResult != null)
			//{
			//	var urlBase = Request.RequestUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
			//	var result = new ImportableListObject
			//	{
			//		ItemCount = jobResult.ImportableFiles.Count,
			//		ImportablePhotos = jobResult.ImportableFiles.Select(x => new ImportableItem
			//		{
			//			FullPath = x,
			//			Filename = x.Split('/').Last(),
			//			ThumbUrl = $"{urlBase}/api/image?path={Uri.EscapeDataString(x)}&size=200"
			//		}).ToList()
			//	};

			//	return Request.CreateResponse(HttpStatusCode.OK, result);
			//}

			return new ObjectResult("Job complete without error, but no result available") { StatusCode = (int)HttpStatusCode.InternalServerError };
		}
	}
}
