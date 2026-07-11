using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class Feature
	{
		public Feature()
		{
			this.tags = new List<string>();
			this.excludesTags = new List<string>();
		}

		public string type { get; set; }

		public List<string> tags { get; private set; }

		public List<string> excludesTags { get; private set; }
	}
}
