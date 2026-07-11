using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class DefaultSettings
	{
		public DefaultSettings()
		{
			this.data = new Dictionary<string, object>();
			this.defaultMoveTags = new List<string>();
			this.overworldAddTags = new List<string>();
		}

		public BaseLocation baseData { get; private set; }

		public Dictionary<string, object> data { get; private set; }

		public List<string> defaultMoveTags { get; private set; }

		public List<string> overworldAddTags { get; private set; }
	}
}
