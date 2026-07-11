using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.CustomSettings
{
	public class ListSettingConfig : SettingConfig
	{
		public ListSettingConfig(string id, string label, string tooltip, List<SettingLevel> levels, string default_level_id, string nosweat_default_level_id, bool debug_only)
			: base(id, label, tooltip, default_level_id, nosweat_default_level_id, debug_only)
		{
			this.levels = levels;
		}

		public List<SettingLevel> levels { get; private set; }

		public void StompLevels(List<SettingLevel> levels, string default_level_id, string nosweat_default_level_id)
		{
			this.levels = levels;
			base.default_level_id = default_level_id;
			base.nosweat_default_level_id = nosweat_default_level_id;
		}

		public override SettingLevel GetLevel(string level_id)
		{
			for (int i = 0; i < this.levels.Count; i++)
			{
				if (this.levels[i].id == level_id)
				{
					return this.levels[i];
				}
			}
			for (int j = 0; j < this.levels.Count; j++)
			{
				if (this.levels[j].id == base.default_level_id)
				{
					return this.levels[j];
				}
			}
			global::Debug.LogError("Unable to find setting level for setting:" + base.id + " level: " + level_id, null);
			return null;
		}

		public string CycleSettingLevelID(string current_id, int direction)
		{
			string text = string.Empty;
			if (current_id == string.Empty)
			{
				current_id = this.levels[0].id;
			}
			for (int i = 0; i < this.levels.Count; i++)
			{
				if (this.levels[i].id == current_id)
				{
					int num = Mathf.Clamp(i + direction, 0, this.levels.Count - 1);
					text = this.levels[num].id;
					break;
				}
			}
			return text;
		}

		public bool IsFirstLevel(string level_id)
		{
			return this.levels.FindIndex((SettingLevel l) => l.id == level_id) == 0;
		}

		public bool IsLastLevel(string level_id)
		{
			return this.levels.FindIndex((SettingLevel l) => l.id == level_id) == this.levels.Count - 1;
		}
	}
}
