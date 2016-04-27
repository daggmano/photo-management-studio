using System.IO;
using System.Net;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using System.Threading.Tasks;
using PhotoLibraryImageService.Helpers;
using PhotoLibraryImageService.Services;
using PhotoLibraryImageService.Data.Interfaces;
using Shared;

namespace PhotoLibraryImageService.Controllers
{
	[Route("api/[controller]")]
	public class ImageController : Controller
	{
		private readonly AppSettings _appSettings;
		private readonly IApplicationEnvironment _appEnvironment;
		private readonly IDataService _dataService;

		public ImageController(IOptions<AppSettings> options, IApplicationEnvironment appEnvironment, IDataService dataService)
		{
			_appSettings = options.Value;
			_appEnvironment = appEnvironment;
			_dataService = dataService;
		}
		
		[Route("{uid}")]
		public async Task<IActionResult> GetFromDb(string uid, int size, int w, int h)
		{
			System.Console.WriteLine("ID is " + uid);
			System.Console.WriteLine("Thumbnail Folder is " + _appSettings.ThumbnailPath);
			if (!Directory.Exists(_appSettings.ThumbnailPath))
			{
				Directory.CreateDirectory(_appSettings.ThumbnailPath);
			}
			
			if (w == 0 || h == 0)
			{
				w = size;
				h = size;
			}
			
			var thumbnail = Path.Combine(_appSettings.ThumbnailPath, $"{uid}_{w}x{h}.jpg");
			if (System.IO.File.Exists(thumbnail))
			{
				var bytes = System.IO.File.ReadAllBytes(thumbnail);
				var ms = new MemoryStream(bytes);
				ms.Seek(0, SeekOrigin.Begin);
				return new FileResultFromStream(Path.GetFileName(thumbnail), ms, "image/jpg");
			}
			
			var photo = await _dataService.GetMedia(uid);


			var rootPath = _appSettings.LibraryPath;
			if (photo != null)
			{
				var path = Path.Combine(rootPath, photo.FullFilePath);
				System.Console.WriteLine("Path is " + path);

//				var hash = Hashing.GetStringSha256Hash($"{path}_{w}x{h}");

//				var cachedImage = Caching.Instance.GetCachedItem<byte[]>(hash);
//				if (cachedImage != null)
//				{
//					var ms = new MemoryStream(cachedImage);
//					return new FileResultFromStream("out.jpg", ms, "image/jpg");
//				}

				if (System.IO.File.Exists(path))
				{
					var placeHolderPath = Path.Combine(_appEnvironment.ApplicationBasePath, "placeholder-1200x1080.jpg");
					var bytes = await ImageResizeService.ProcessImage(path, w, h, placeHolderPath);

//					Caching.Instance.AddToCache(hash, bytes);
					System.IO.File.WriteAllBytes(thumbnail, bytes);

					var ms = new MemoryStream(bytes);
					return new FileResultFromStream(Path.GetFileName(thumbnail), ms, "image/jpg");
				}
			}

			return new ObjectResult("Image not found: " + uid) { StatusCode = (int)HttpStatusCode.NotFound };
		}

		public async Task<IActionResult> Get(string path, int size, int w, int h)
		{
			System.Console.WriteLine("Folder is " + _appEnvironment.ApplicationBasePath);
			if (string.IsNullOrWhiteSpace(path))
			{
				return new ObjectResult("Have not provided this for non-temporary files yet") { StatusCode = (int)HttpStatusCode.Forbidden };
			}

			if (w == 0 || h == 0)
			{
				w = size;
				h = size;
			}

			var rootPath = _appSettings.LibraryPath;
			path = Path.Combine(rootPath, path);

			var hash = Hashing.GetStringSha256Hash($"{path}_{w}x{h}");

			var cachedImage = Caching.Instance.GetCachedItem<byte[]>(hash);
			if (cachedImage != null)
			{
				var ms = new MemoryStream(cachedImage);
				return new FileResultFromStream("out.jpg", ms, "image/jpg");
			}

			if (System.IO.File.Exists(path))
			{
				var placeHolderPath = Path.Combine(_appEnvironment.ApplicationBasePath, "placeholder-1200x1080.jpg");
				var bytes = await ImageResizeService.ProcessImage(path, w, h, placeHolderPath);

				Caching.Instance.AddToCache(hash, bytes);

				var ms = new MemoryStream(bytes);
				return new FileResultFromStream("out.jpg", ms, "image/jpg");
			}

			return new ObjectResult("Image not found: " + path) { StatusCode = (int)HttpStatusCode.NotFound };
		}
	}
}
