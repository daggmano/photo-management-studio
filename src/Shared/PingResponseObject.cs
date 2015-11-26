using System;

namespace Shared
{
	public class PingResponseObject : ResponseObject<PingResponseData>
	{
	}

	public class PingResponseData
	{
		public DateTime ServerDateTime { get; set; }
	}
}
