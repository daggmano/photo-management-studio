using System.Collections.Generic;

namespace Shared
{
	public class ImportableListObject
	{
		public int ItemCount { get; set; }
		public List<ImportableItem> ImportablePhotos { get; set; }
	}

	public class ImportableItem
	{
		public string Filename { get; set; }
		public string FullPath { get; set; }
		public string ThumbUrl { get; set; }
	}
}
