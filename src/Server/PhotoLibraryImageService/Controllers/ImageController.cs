using System.IO;
using System.Net;
using Microsoft.AspNet.Mvc;
using PhotoLibraryImageService.Services;
using Shared;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using System.Threading.Tasks;

namespace PhotoLibraryImageService.Controllers
{
	[Route("api/[controller]")]
	public class ImageController : Controller
	{
		private readonly AppSettings _appSettings;
		private readonly IApplicationEnvironment _appEnvironment;

		public ImageController(IOptions<AppSettings> options, IApplicationEnvironment appEnvironment)
		{
			_appSettings = options.Value;
			_appEnvironment = appEnvironment;
		}

		public async Task<IActionResult> Get(string path, int size)
		{
			System.Console.WriteLine("Folder is " + _appEnvironment.ApplicationBasePath);
			if (string.IsNullOrWhiteSpace(path))
			{
				return new ObjectResult("Have not provided this for non-temporary files yet") { StatusCode = (int)HttpStatusCode.Forbidden };
			}

			var rootPath = _appSettings.LibraryPath;
			path = Path.Combine(rootPath, path);
			if (System.IO.File.Exists(path))
			{
				var placeHolderPath = Path.Combine(_appEnvironment.ApplicationBasePath, "placeholder-1200x1080.jpg");
				var bytes = await ImageResizeService.ProcessImage(path, size, placeHolderPath);

				var ms = new MemoryStream(bytes);
				return new FileResultFromStream("out.jpg", ms, "image/jpg");
			}

//            var localId = Path.GetFileNameWithoutExtension(id);

//            var imgName = localId + "_" + size + ".jpg";
//            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
//            if (filePath == null)
//            {
//                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Unable to find Assembly folder.");
//            }

//            filePath = Path.Combine(filePath, "App_Data", imgName);
//            if (File.Exists(filePath))
//            {
//                var result = new HttpResponseMessage(HttpStatusCode.OK);
//                var stream = new FileStream(filePath, FileMode.Open);
//                result.Content = new StreamContent(stream);
//                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
//                return result;
//            }
//            else
//            {
//                ImageResizeService.ProcessImage(id, size, filePath);
//            }

			return new ObjectResult("Image not found: " + path) { StatusCode = (int)HttpStatusCode.NotFound };
		}
	}
}
