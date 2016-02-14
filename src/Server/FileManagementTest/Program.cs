using System;
using System.Linq;
using FileManager;
using Newtonsoft.Json;

namespace FileManagementTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Process();

			Console.WriteLine();
			Console.ReadLine();
		}

		private static async void Process()
		{
			const string folder = @"/Users/darrenoster/Desktop/TestImages";

			var service = new FileManagementService();

			var files = service.GetFileList(folder).Select(x => x.Substring(1).ToLowerInvariant().Replace("\\", "/")).ToList();
			var media = await service.GetAllPhotoPaths();

			var mediaFileNames = media.Select(x => x.MediaId);

			var missingFiles = files.Where(x => !mediaFileNames.Contains(x));

//            var file = missingFiles.FirstOrDefault(x => Path.GetExtension(x).ToLowerInvariant().Equals(".cr2"));

			var processor = new FileProcessor();

			foreach (var file in missingFiles.Where(x => x != ".ds_store"))
			{
				var mediaObject = processor.ProcessFile(file, folder, Guid.NewGuid());

				Console.WriteLine(file);
				Console.WriteLine();
				Console.WriteLine(JsonConvert.SerializeObject(mediaObject));
				Console.WriteLine();
				Console.WriteLine();
			}
		}
	}
}
