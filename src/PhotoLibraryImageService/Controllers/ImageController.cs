using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using PhotoLibraryImageService.Services;
using System.Configuration;

namespace PhotoLibraryImageService.Controllers
{
    public class ImageController : ApiController
    {
//		public HttpResponseMessage Get(string id, int size)
        public HttpResponseMessage GetPath(string path, int size)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Have not provided this for non-temporary files yet");
			}

			var rootPath = ConfigurationManager.AppSettings["LibraryPath"];
			path = path.Replace("/", "\\");
			path = Path.Combine(rootPath, path);

			if (File.Exists(path))
			{
				var bytes = ImageResizeService.ProcessImage(path, size);
				if (bytes != null)
				{
					var result = new HttpResponseMessage(HttpStatusCode.OK);
					var stream = new MemoryStream(bytes);
					result.Content = new StreamContent(stream);
					result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
					return result;
				}
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

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Image not found");
        }
    }
}
