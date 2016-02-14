using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using PhotoLibraryImageService.Data;
using PhotoLibraryImageService.Data.Interfaces;
using ErrorReporting;
using Shared;

namespace PhotoLibraryImageService
{
	public class Startup
	{
		private readonly int _listenPort;
		private readonly int _photoServerPort;

		public Startup(IHostingEnvironment env)
		{
			// Set up configuration sources.
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appSettings.json")
				.AddEnvironmentVariables();
			Configuration = builder.Build();

			_listenPort = SharedConfiguration.GetAppSettings().UdpListenPort;
			_photoServerPort = SharedConfiguration.GetAppSettings().PhotoServerPort;

			var listenerThread = new Thread(StartListener);
			listenerThread.IsBackground = true;
			listenerThread.Start();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc();

			services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
			services.AddScoped<IDataService, DataService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.MinimumLevel = LogLevel.Debug;
			loggerFactory.AddConsole();
			loggerFactory.AddDebug();

			app.UseIISPlatformHandler();
			app.UseStaticFiles();
			app.UseMvc();
		}

		// Entry point for the application.
		public static void Main(string[] args) => Microsoft.AspNet.Hosting.WebApplication.Run<Startup>(args);

		private void StartListener()
		{
			var listener = new UdpClient(_listenPort);
			var groupEp = new IPEndPoint(IPAddress.Any, _listenPort);

			try
			{
				while (true)
				{
					Console.WriteLine($"Waiting for broadcast on port {_listenPort}, {groupEp}");
					var bytes = listener.Receive(ref groupEp);
					Console.WriteLine("Got one");
					var s = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
					Console.WriteLine(s);
					var discoveryObject = JsonConvert.DeserializeObject<NetworkDiscoveryObject>(s);
					if (discoveryObject != null)
					{
						Console.WriteLine("IP Address: " + groupEp.Address);
						Console.WriteLine("Socket Port: " + discoveryObject.ClientSocketPort);

						var localIpAddress = GetLocalIpAddress();

						var serverSpecification = new ServerSpecificationObject
						{
							ServerAddress = localIpAddress,
							ServerPort = _photoServerPort
						};
						var networkMessage = new NetworkMessageObject<ServerSpecificationObject>
						{
							MessageType = NetworkMessageType.ServerSpecification,
							Message = serverSpecification
						};

						SocketClient.Send(groupEp.Address, discoveryObject.ClientSocketPort, networkMessage);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				ErrorReporter.SendException(e);
			}
			finally
			{
				listener.Close();
				Console.WriteLine("UDP Listener closed");
			}
		}

		public static string GetLocalIpAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			throw new Exception("Local IP Address Not Found!");
		}
	}
}


/*
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Shared;
using ErrorReporting;

namespace PhotoLibraryImageService
{
	internal class Program
	{
		private static readonly int ListenPort = Int32.Parse(ConfigurationManager.AppSettings["UdpListenPort"]);
		private static readonly int PhotoServerPort = Int32.Parse(ConfigurationManager.AppSettings["PhotoServerPort"]);

		private static void Main(string[] args)
		{
			var listenerThread = new Thread(StartListener);
			listenerThread.Start();

			StartWebApi();
		}

		private static async void StartWebApi()
		{
			Func<IPAddress, string> createIpString = (a) => String.Format("http://{0}:{1}", a, PhotoServerPort);

			var ipAddresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
			var options = new StartOptions(createIpString(IPAddress.Loopback));
			foreach (var address in ipAddresses)
			{
				options.Urls.Add(createIpString(address));
			}

			// Start OWIN host
			using (WebApp.Start<Startup>(options))
			{
				while (true)
				{
					await Task.Delay(10);
				}
			}
		}

		private static void StartListener()
		{
			var listener = new UdpClient(ListenPort);
			var groupEp = new IPEndPoint(IPAddress.Any, ListenPort);

			try
			{
				while (true)
				{
					Console.WriteLine("Waiting for broadcast");
					var bytes = listener.Receive(ref groupEp);
					var s = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
					var discoveryObject = JsonConvert.DeserializeObject<NetworkDiscoveryObject>(s);
					if (discoveryObject != null)
					{
						Console.WriteLine("IP Address: " + groupEp.Address);
						Console.WriteLine("Socket Port: " + discoveryObject.ClientSocketPort);

						var serverSpecification = new ServerSpecificationObject
						{
							ServerAddress = "127.0.0.1",
							ServerPort = PhotoServerPort
						};
						var networkMessage = new NetworkMessageObject<ServerSpecificationObject>
						{
							MessageType = NetworkMessageType.ServerSpecification,
							Message = serverSpecification
						};

						SocketClient.Send(groupEp.Address, discoveryObject.ClientSocketPort, networkMessage);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				ErrorReporter.SendException(e);
			}
			finally
			{
				listener.Close();
			}
		}
	}
}
*/
