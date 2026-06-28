using System;
using System.Collections.Generic;
using Klei;

namespace ProcGen
{
	public class DefaultSettings : YamlIO<DefaultSettings>
	{
		public DefaultSettings()
		{
			this.baseData = new BaseLocation();
			this.data = new Dictionary<string, object>();
			this.cloudSettings = new CloudSettings();
		}

		public BaseLocation baseData { get; private set; }

		public Dictionary<string, object> data { get; private set; }

		public List<string> defaultMoveTags { get; private set; }

		public List<string> overworldAddTags { get; private set; }

		public CloudSettings cloudSettings { get; private set; }
	}
}
