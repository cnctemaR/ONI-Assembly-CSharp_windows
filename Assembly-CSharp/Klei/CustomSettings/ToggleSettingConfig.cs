using System;

namespace Klei.CustomSettings
{
	public class ToggleSettingConfig : SettingConfig
	{
		public ToggleSettingConfig(string id, string label, string tooltip, SettingLevel off_level, SettingLevel on_level, string default_level_id)
			: base(id, label, tooltip, default_level_id)
		{
			this.off_level = off_level;
			this.on_level = on_level;
		}

		public SettingLevel on_level { get; private set; }

		public SettingLevel off_level { get; private set; }

		public override SettingLevel GetLevel(string level_id)
		{
			SettingLevel settingLevel;
			if (this.on_level.id == level_id)
			{
				settingLevel = this.on_level;
			}
			else if (this.off_level.id == level_id)
			{
				settingLevel = this.off_level;
			}
			else if (base.default_level_id == this.on_level.id)
			{
				Debug.LogWarning(string.Concat(new string[] { "Unable to find level for setting:", base.id, "(", level_id, ") Using default level." }), null);
				settingLevel = this.on_level;
			}
			else if (base.default_level_id == this.off_level.id)
			{
				Debug.LogWarning(string.Concat(new string[] { "Unable to find level for setting:", base.id, "(", level_id, ") Using default level." }), null);
				settingLevel = this.off_level;
			}
			else
			{
				Debug.LogError("Unable to find setting level for setting:" + base.id + " level: " + level_id, null);
				settingLevel = null;
			}
			return settingLevel;
		}

		public string ToggleSettingLevelID(string current_id)
		{
			string text;
			if (this.on_level.id == current_id)
			{
				text = this.off_level.id;
			}
			else
			{
				text = this.on_level.id;
			}
			return text;
		}

		public bool IsOnLevel(string level_id)
		{
			return level_id == this.on_level.id;
		}
	}
}
