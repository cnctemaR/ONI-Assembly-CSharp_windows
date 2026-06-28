using System;

namespace Klei.CustomSettings
{
	public class SeedSettingConfig : SettingConfig
	{
		public SeedSettingConfig(string id, string label, string tooltip)
			: base(id, label, tooltip, string.Empty)
		{
		}

		public override SettingLevel GetLevel(string level_id)
		{
			return null;
		}
	}
}
