using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PhotoLibraryImageService.Data.Interfaces;

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

            return serverId != null
                ? Request.CreateResponse(HttpStatusCode.OK, serverId)
                : Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Unable to find server identification.");
        }
    }
}
