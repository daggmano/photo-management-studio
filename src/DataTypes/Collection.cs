using System.Collections.Generic;

namespace DataTypes
{
	public class Collection
	{
		public string CollectionId { get; set; }
		public string CollectionRev { get; set; }

		public string Name { get; set; }
		public List<string> MediaIds { get; set; }
	}
}
