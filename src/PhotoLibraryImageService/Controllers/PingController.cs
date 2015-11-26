using PhotoLibraryImageService.Helpers;
using Shared;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace PhotoLibraryImageService.Controllers
{
    public class PingController : ApiController
    {
        public HttpResponseMessage Get()
        {
			var data = new PingResponseObject
			{
				Links = new LinksObject
				{
					Self = Request.GetSelfLink()
				},
				Data = new PingResponseData
				{
					ServerDateTime = DateTime.UtcNow
				}
			};

			var response = Request.CreateResponse(HttpStatusCode.OK, data);
			response.Headers.CacheControl = new CacheControlHeaderValue
			{
				NoStore = true,
				Public = false,
				MaxAge = new TimeSpan(0, 0, 0, 0)
			};
			return response;
        }
	}
}
