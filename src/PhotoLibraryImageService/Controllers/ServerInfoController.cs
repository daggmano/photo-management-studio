using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PhotoLibraryImageService.Data.Interfaces;
using Shared;
using PhotoLibraryImageService.Helpers;
using System.Net.Http.Headers;
using System;

namespace PhotoLibraryImageService.Controllers
{
    public class ServerInfoController : ApiController
    {
        private readonly IDataService _dataService;

        public ServerInfoController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<HttpResponseMessage> Get()
        {
            var serverId = await _dataService.GetServerDatabaseIdentifier();

			if (serverId == null)
			{
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Unable to find server identification.");
			}

			var data = new ServerInfoResponseObject
			{
				Links = new LinksObject
				{
					Self = Request.GetSelfLink()
				},
				Data = serverId
			};

			var response = Request.CreateResponse(HttpStatusCode.OK, data);
			response.Headers.CacheControl = new CacheControlHeaderValue
			{
				Public = true,
				MaxAge = new TimeSpan(1, 0, 0, 0)
			};
			return response;
		}
    }
}
