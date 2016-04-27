using System;
using System.Collections.Generic;

namespace DataTypes
{
	public class Media
 	{
		/// <summary>
		/// MediaId is the Lowered File Path
		/// </summary>
		public string MediaId { get; set; }
		public string MediaRev { get; set; }

		public string ImportId { get; set; }
		public string UniqueId { get; set; }

		public string FullFilePath { get; set; }
		public string FileName { get; set; }
		public DateTime ShotDate { get; set; }
		public int Rating { get; set; }
		public int DateAccuracy { get; set; }
		public string Caption { get; set; }
		public int Rotate { get; set; }
		public List<Metadata> Metadata { get; set; }
		public List<string> TagIds { get; set; }

		public Media()
		{
			Metadata = new List<Metadata>();
			TagIds = new List<string>();
		}
	}

	public class Metadata
	{
		public string Group { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
	}
}
