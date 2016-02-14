using System.Collections.Generic;
using System.Linq;

namespace Shared
{
	public enum ErrorTypes
	{
		UnableToConnectToDatabase,
		MissingDatabase
	}

	public class ErrorMessageReponse
	{
		public int Code { get; set; }
		public string Message { get; set; }
		public string Description { get; set; }
	}

	public static class Errors
	{
		private class ErrorListItem
		{
			public ErrorTypes ErrorType { get; set; }
			public int Code { get; set; }
			public string Message { get; set; }
			public string Description { get; set; }

			public ErrorListItem(ErrorTypes errorType, int code, string message, string description)
			{
				ErrorType = errorType;
				Code = code;
				Message = message;
				Description = description;
			}
		}

		private static readonly List<ErrorListItem> _errorList = new List<ErrorListItem>
		{
			new ErrorListItem(ErrorTypes.UnableToConnectToDatabase, 1001, "Unable to connect to database", "Unable to connect to the database.  Is CouchDB running on the server?"),
			new ErrorListItem(ErrorTypes.MissingDatabase, 1002, "Unable to find database", "Unable to find 'photos' database on server or the database is incorrectly configured.  Perhaps you need to run the Setup program?")
		};

		public static ErrorMessageReponse GetErrorResponse(ErrorTypes errorType)
		{
			var item = _errorList.SingleOrDefault(x => x.ErrorType == errorType);
			return new ErrorMessageReponse
			{
				Code = item?.Code ?? -1,
				Message = item?.Message ?? "Unknown Error",
				Description = item?.Description ?? "Unknown error. Please inform the developer."
			};
		}

		public static string GetErrorDescription(ErrorTypes errorType)
		{
			return _errorList.SingleOrDefault(x => x.ErrorType == errorType)?.Description ?? "Unknown error. Please inform the developer.";
		}
	}
}
