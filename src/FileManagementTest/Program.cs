using System;
using System.Linq;
using FileManager;

namespace FileManagementTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Process();

            Console.WriteLine();
            Console.ReadLine();
        }

        private static async void Process()
        {
            const string folder = @"\\MediaCenter\Photos\";

            var service = new FileManagementService();

            var files = service.GetFileList(folder);
            var media = await service.GetAllPhotoPaths();

            var mediaFileNames = media.Select(x => x.LoweredFilePath);

            var missingFiles = files.Where(x => !mediaFileNames.Contains(x.ToLowerInvariant()));

            foreach (var file in missingFiles)
            {
                Console.WriteLine(file);
            }
        }
    }
}
