using System;

namespace Klei.CustomSettings
{
	public class SettingLevel
	{
		public SettingLevel(string id, string label, string tooltip, long coordinate_offset = 0L, object userdata = null)
		{
			this.id = id;
			this.label = label;
			this.tooltip = tooltip;
			this.userdata = userdata;
			this.coordinate_offset = coordinate_offset;
		}

		public string id { get; private set; }

		public string tooltip { get; private set; }

		public string label { get; private set; }

		public object userdata { get; private set; }

		public long coordinate_offset { get; private set; }
	}
}
