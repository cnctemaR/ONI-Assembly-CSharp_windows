using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.CustomSettings
{
	public class ListSettingConfig : SettingConfig
	{
		public List<SettingLevel> levels { get; private set; }

		public ListSettingConfig(string id, string label, string tooltip, List<SettingLevel> levels, string default_level_id, string nosweat_default_level_id, long coordinate_range = -1L, bool debug_only = false, bool triggers_custom_game = true, string[] required_content = null, string missing_content_default = "", bool hide_in_ui = false)
			: base(id, label, tooltip, default_level_id, nosweat_default_level_id, coordinate_range, debug_only, triggers_custom_game, required_content, missing_content_default, hide_in_ui)
		{
			this.levels = levels;
		}

		public void StompLevels(List<SettingLevel> levels, string default_level_id, string nosweat_default_level_id)
		{
			this.levels = levels;
			this.default_level_id = default_level_id;
			this.nosweat_default_level_id = nosweat_default_level_id;
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
				if (this.levels[j].id == this.default_level_id)
				{
					return this.levels[j];
				}
			}
			global::Debug.LogError("Unable to find setting level for setting:" + base.id + " level: " + level_id);
			return null;
		}

		public override List<SettingLevel> GetLevels()
		{
			return this.levels;
		}

		public string CycleSettingLevelID(string current_id, int direction)
		{
			string text = "";
			if (current_id == "")
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
