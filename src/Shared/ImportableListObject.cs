using System.Collections.Generic;

namespace Shared
{
	public class ImportableListObject
	{
		public List<string> ImportableFiles { get; set; }
		public List<string> MissingFiles { get; set; }
	}
}
