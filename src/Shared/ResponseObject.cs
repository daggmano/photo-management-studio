using System.Collections.Generic;

namespace Shared
{
	public abstract class ResponseObject<T>
	{
		public LinksObject Links { get; set; }
		public T Data { get; set; }
	}

	public abstract class ResponseListObject<T>
	{
		public LinksObject Links { get; set; }
		public List<T> Data { get; set; }
	}
}
