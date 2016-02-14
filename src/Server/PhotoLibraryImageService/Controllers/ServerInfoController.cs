using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PhotoLibraryImageService.Data.Interfaces;
using Shared;
using PhotoLibraryImageService.Helpers;
using System;
using ErrorReporting;
using System.Net.Sockets;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;

namespace PhotoLibraryImageService.Controllers
{
	[Route("api/[controller]")]
	public class ServerInfoController : Controller
	{
		private readonly IDataService _dataService;
		private readonly AppSettings _appSettings;

		public ServerInfoController(IDataService dataService, IOptions<AppSettings> options)
		{
			_dataService = dataService;
			_appSettings = options.Value;
		}

		public async Task<IActionResult> Get()
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
				return new ObjectResult(Errors.GetErrorResponse(ErrorTypes.MissingDatabase)) { StatusCode = (int)HttpStatusCode.InternalServerError };
			}
			catch (Exception ex)
			{
				if (ex is HttpRequestException && ex.InnerException != null && ex.InnerException is WebException)
				{
					var inner = ex.InnerException;
					if (inner.InnerException != null && inner.InnerException is SocketException)
					{
						return new ObjectResult(Errors.GetErrorResponse(ErrorTypes.UnableToConnectToDatabase)) { StatusCode = (int)HttpStatusCode.InternalServerError };
					}
				}

				ErrorReporter.SendException(ex);
				return new ObjectResult("Unable to find server identification.") { StatusCode = (int)HttpStatusCode.InternalServerError };
			}

			if (serverId == null)
			{
				return new ObjectResult("Unable to find server identification.") { StatusCode = (int)HttpStatusCode.InternalServerError };
			}

			var data = new ServerInfoResponseObject
			{
				Links = new LinksObject
				{
					Self = Request.GetSelfLink()
				},
				Data = serverId
			};

			return new ObjectResult(data);
		}
	}
}
