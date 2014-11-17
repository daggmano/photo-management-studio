using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PhotoLibraryImageService.Controllers
{
    public class PingController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new {ServerDateTime = DateTime.Now});
        }
    }
}
