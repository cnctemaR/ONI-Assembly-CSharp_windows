using System;

namespace Klei.CustomSettings
{
	public class SettingLevel
	{
		public SettingLevel(string id, string label, string tooltip)
		{
			this.id = id;
			this.label = label;
			this.tooltip = tooltip;
		}

		public string id { get; private set; }

		public string tooltip { get; private set; }

		public string label { get; private set; }
	}
}
