using System;
using System.Collections.Generic;
using Klei;

namespace ProcGen
{
	public class DefaultSettings : YamlIO<DefaultSettings>
	{
		public DefaultSettings()
		{
			this.data = new Dictionary<string, object>();
		}

		public BaseLocation baseData { get; private set; }

		public Dictionary<string, object> data { get; private set; }

		public List<string> defaultMoveTags { get; private set; }

		public List<string> overworldAddTags { get; private set; }
	}
}
