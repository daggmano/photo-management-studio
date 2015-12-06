using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PhotoLibraryImageService.Data.Interfaces;
using Shared;
using PhotoLibraryImageService.Helpers;
using System.Net.Http.Headers;
using System;
using ErrorReporting;
using System.Net.Sockets;

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
			ServerDatabaseIdentifierObject serverId = null;
			try
			{
				var serverDetails = await _dataService.GetServerDatabaseIdentifier();
				serverId = new ServerDatabaseIdentifierObject
				{
					ServerId = serverDetails.ServerId,
					ServerName = serverDetails.ServerName
				};
			}
			catch (ArgumentNullException)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, Errors.GetErrorResponse(ErrorTypes.MissingDatabase));
			}
			catch (Exception ex)
			{
				if (ex is HttpRequestException && ex.InnerException != null && ex.InnerException is WebException)
				{
					var inner = ex.InnerException;
					if (inner.InnerException != null && inner.InnerException is SocketException)
					{
						return Request.CreateResponse(HttpStatusCode.InternalServerError, Errors.GetErrorResponse(ErrorTypes.UnableToConnectToDatabase));
					}
				}

				ErrorReporter.SendException(ex);
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Unable to find server identification.");
			}

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
