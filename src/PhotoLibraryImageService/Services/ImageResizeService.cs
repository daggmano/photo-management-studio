using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PhotoLibraryImageService.Services
{
    public class ImageResizeService
    {
//        public static void ProcessImage(string imageId, int maxDimension, string outPath)
//        {
//            var path = Path.Combine(SettingsService.GetLibraryPath(), imageId);

//            if (!File.Exists(path))
//            {
//                return;
//            }

//            using (var image = new MagickImage(path))
//            {
//                image.Resize(maxDimension, maxDimension);
//                image.Write(outPath);
//            }
//        }

		public async static Task<byte[]> ProcessImage(string path, int maxDimension, string placeholderPath)
		{
			try {
				Console.WriteLine($"Path: {path}");
				if (!File.Exists(path))
				{
					return null;
				}
				
				var psi = new ProcessStartInfo();
				psi.FileName = "convert";
				psi.Arguments = $"\"{path}\" -resize {maxDimension}x{maxDimension} jpeg:-";
				
				var ms = await ReadProcessOutput(psi);
				
				var result = ms.ToArray();
				if (result.Length == 0) {
					throw new Exception();
				}
				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex.Message}");

				var processStartInfo = new ProcessStartInfo();
				processStartInfo.FileName = "convert";
				processStartInfo.Arguments = $"\"{placeholderPath}\" -resize {maxDimension}x{maxDimension} jpeg:-";
				
				var ms = await ReadProcessOutput(processStartInfo);
				
				var result = ms.ToArray();
				Console.WriteLine($"Result Length: {result.Length}");
				return result;
			}
		}
		
		private static async Task<MemoryStream> ReadProcessOutput(ProcessStartInfo psi)
		{
    		MemoryStream ms = new MemoryStream();
			
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			psi.UseShellExecute = false;

    		using (Process p = new Process())
    		{
        		p.StartInfo = psi;

        		TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
        		EventHandler eh = (s, e) => tcs.TrySetResult(0);

        		p.Exited += eh;

        		try
        		{
            		p.EnableRaisingEvents = true;
            		p.Start();

            		await p.StandardOutput.BaseStream.CopyToAsync(ms);
            		await tcs.Task;
        		}
				catch (Exception ex)
				{
					Console.WriteLine($"RPO Exception: {ex.Message}");
				}
        		finally
        		{
            		p.Exited -= eh;
        		}
    		}

    		return ms;
		}
    }
}
