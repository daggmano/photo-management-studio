using PhotoLibraryImageService.Jobs;
using PhotoLibraryImageService.Services;
using Shared;
using System;
using System.Net;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;

namespace PhotoLibraryImageService.Controllers
{
	public class ImportController : Controller
	{
		private readonly IOptions<AppSettings> _appSettings;

		public ImportController(IOptions<AppSettings> appSettings)
		{
			_appSettings = appSettings;
		}

		[HttpPost]
		[Route("api/import")]
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
		[Route("api/import/{id}")]
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
					return new ObjectResult("Job is in error state") { StatusCode = (int)HttpStatusCode.InternalServerError };
				case JobStates.Running:
					return new ObjectResult("Job is still running") { StatusCode = (int)HttpStatusCode.PreconditionFailed };
				case JobStates.Submitted:
					return new ObjectResult("Job has not started yet") { StatusCode = (int)HttpStatusCode.PreconditionFailed };
				case JobStates.Unknown:
					return new ObjectResult("Job is in an unknown state") { StatusCode = (int)HttpStatusCode.ExpectationFailed };
				default:
					break;
			}

			var jobResult = JobsService.GetInstance().GetResult<ImportJobResult>(id);

			if (jobResult != null)
			{
				var result = new ImportedFilesObject
				{
					ItemCount = jobResult.ImportedFiles.Count,
					ImportedPhotos = jobResult.ImportedFiles
				};

				Console.WriteLine($"ItemCount: {result.ItemCount}");

				return new ObjectResult(result);
			}

			return new ObjectResult("Job complete without error, but no result available") { StatusCode = (int)HttpStatusCode.OK };
		}
	}
}
