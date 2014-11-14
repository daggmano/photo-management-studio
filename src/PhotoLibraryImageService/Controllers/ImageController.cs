using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using PhotoLibraryImageService.Services;

namespace PhotoLibraryImageService.Controllers
{
    public class ImageController : ApiController
    {
        public HttpResponseMessage Get(string id, int size)
        {
            var localId = Path.GetFileNameWithoutExtension(id);

            var imgName = localId + "_" + size + ".jpg";
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (filePath == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Unable to find Assembly folder.");
            }

            filePath = Path.Combine(filePath, "App_Data", imgName);
            if (File.Exists(filePath))
            {
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(filePath, FileMode.Open);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                return result;
            }
            else
            {
                ImageResizeService.ProcessImage(id, size, filePath);
            }


            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Image not found");
        }
    }
}
