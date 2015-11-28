using ErrorReporting;
using Shared;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Setup
{
	class Program
	{
		static void Main(string[] args)
		{
			var initializer = new DatabaseInitializer();

			try
			{
				var task = initializer.Run();
				Task.WaitAll(task);
			}
			catch (Exception ex)
			{
				if (ex is HttpRequestException && ex.InnerException != null && ex.InnerException is WebException)
				{
					var inner = ex.InnerException;
					if (inner.InnerException != null && inner.InnerException is SocketException)
					{
						Console.WriteLine(Errors.GetErrorDescription(ErrorTypes.UnableToConnectToDatabase));
					}
				}

				ErrorReporter.SendException(ex);
				Console.WriteLine($"An unknown error occurred: {ex.Message}");
			}

			Console.WriteLine("Press <Enter> to close...");
			Console.ReadLine();
		}
	}
}
