using RollbarSharp;
using System;
using Shared;

namespace ErrorReporting
{
	public class ErrorReporter
	{
		private static Configuration _configuration;
		
		static ErrorReporter()
		{
			var appSettings = SharedConfiguration.GetAppSettings();
			_configuration = new Configuration(appSettings.Rollbar.AccessToken) {
				Environment = appSettings.Rollbar.Environment
			};
		}
		
		public static void SendException(Exception ex)
		{
			(new RollbarClient(_configuration)).SendException(ex);
		}

		public static void SendCriticalException(Exception ex)
		{
			(new RollbarClient(_configuration)).SendCriticalException(ex);
		}

		public static void SendErrorException(Exception ex)
		{
			(new RollbarClient(_configuration)).SendErrorException(ex);
		}

		public static void SendWarningException(Exception ex)
		{
			(new RollbarClient(_configuration)).SendWarningException(ex);
		}

		public static void SendMessage(string msg, string level)
		{
			(new RollbarClient(_configuration)).SendMessage(msg, level);
		}

		public static void SendCriticalMessage(string msg)
		{
			(new RollbarClient(_configuration)).SendCriticalMessage(msg);
		}

		public static void SendDebugMessage(string msg)
		{
			(new RollbarClient(_configuration)).SendDebugMessage(msg);
		}

		public static void SendErrorMessage(string msg)
		{
			(new RollbarClient(_configuration)).SendErrorMessage(msg);
		}

		public static void SendInfoMessage(string msg)
		{
			(new RollbarClient(_configuration)).SendInfoMessage(msg);
		}

		public static void SendWarningMessage(string msg)
		{
			(new RollbarClient(_configuration)).SendWarningMessage(msg);
		}
	}
}
