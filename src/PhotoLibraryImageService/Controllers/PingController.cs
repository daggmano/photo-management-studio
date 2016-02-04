using PhotoLibraryImageService.Helpers;
using Shared;
using System;
using Microsoft.AspNet.Mvc;

namespace PhotoLibraryImageService.Controllers
{
	[Route("api/[controller]")]
	public class PingController : Controller
	{
		[HttpGet]
		public IActionResult Get()
		{
			var data = new PingResponseObject
			{
				Links = new LinksObject
				{
					Self = HttpContext.Request.GetSelfLink()
				},
				Data = new PingResponseData
				{
					ServerDateTime = DateTime.UtcNow
				}
			};

			return new ObjectResult(data);
		}
	}
}
