using RollbarSharp;
using System;

namespace ErrorReporting
{
    public class ErrorReporter
    {
		public static void SendException(Exception ex)
		{
			(new RollbarClient()).SendException(ex);
		}

		public static void SendCriticalException(Exception ex)
		{
			(new RollbarClient()).SendCriticalException(ex);
		}

		public static void SendErrorException(Exception ex)
		{
			(new RollbarClient()).SendErrorException(ex);
		}

		public static void SendWarningException(Exception ex)
		{
			(new RollbarClient()).SendWarningException(ex);
		}

		public static void SendMessage(string msg, string level)
		{
			(new RollbarClient()).SendMessage(msg, level);
		}

		public static void SendCriticalMessage(string msg)
		{
			(new RollbarClient()).SendCriticalMessage(msg);
		}

		public static void SendDebugMessage(string msg)
		{
			(new RollbarClient()).SendDebugMessage(msg);
		}

		public static void SendErrorMessage(string msg)
		{
			(new RollbarClient()).SendErrorMessage(msg);
		}

		public static void SendInfoMessage(string msg)
		{
			(new RollbarClient()).SendInfoMessage(msg);
		}

		public static void SendWarningMessage(string msg)
		{
			(new RollbarClient()).SendWarningMessage(msg);
		}
	}
}
