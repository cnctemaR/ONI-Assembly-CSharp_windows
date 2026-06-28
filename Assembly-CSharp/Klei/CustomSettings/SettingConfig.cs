using System;

namespace Klei.CustomSettings
{
	public abstract class SettingConfig
	{
		public SettingConfig(string id, string label, string tooltip, string default_level_id)
		{
			this.id = id;
			this.label = label;
			this.tooltip = tooltip;
			this.default_level_id = default_level_id;
		}

		public string id { get; private set; }

		public string label { get; private set; }

		public string tooltip { get; private set; }

		public string default_level_id { get; private set; }

		public abstract SettingLevel GetLevel(string level_id);

		public bool IsDefaultLevel(string level_id)
		{
			return level_id == this.default_level_id;
		}
	}
}
