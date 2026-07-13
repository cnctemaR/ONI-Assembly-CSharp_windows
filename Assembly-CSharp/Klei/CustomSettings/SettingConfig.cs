using System;
using System.Collections.Generic;

namespace Klei.CustomSettings
{
	public abstract class SettingConfig : IHasDlcRestrictions
	{
		public SettingConfig(string id, string label, string tooltip, string default_level_id, string nosweat_default_level_id, long coordinate_range = -1L, bool debug_only = false, bool triggers_custom_game = true, string[] required_content = null, string missing_content_default = "", bool hide_in_ui = false)
		{
			this.id = id;
			this.label = label;
			this.tooltip = tooltip;
			this.default_level_id = default_level_id;
			this.nosweat_default_level_id = nosweat_default_level_id;
			this.coordinate_range = coordinate_range;
			this.debug_only = debug_only;
			this.triggers_custom_game = triggers_custom_game;
			this.required_content = required_content;
			this.missing_content_default = missing_content_default;
			this.hide_in_ui = hide_in_ui;
		}

		public string id { get; private set; }

		public virtual string label { get; private set; }

		public virtual string tooltip { get; private set; }

		public long coordinate_range { get; protected set; }

		public string[] required_content { get; private set; }

		public string missing_content_default { get; private set; }

		public bool triggers_custom_game { get; protected set; }

		public bool debug_only { get; protected set; }

		public bool hide_in_ui { get; protected set; }

		public abstract SettingLevel GetLevel(string level_id);

		public abstract List<SettingLevel> GetLevels();

		public bool IsDefaultLevel(string level_id)
		{
			return level_id == this.default_level_id;
		}

		public bool ShowInUI()
		{
			return !this.deprecated && !this.hide_in_ui && (!this.debug_only || DebugHandler.enabled) && DlcManager.IsAllContentSubscribed(this.required_content);
		}

		public string GetDefaultLevelId()
		{
			if (!DlcManager.IsAllContentSubscribed(this.required_content) && !string.IsNullOrEmpty(this.missing_content_default))
			{
				return this.missing_content_default;
			}
			return this.default_level_id;
		}

		public string GetNoSweatDefaultLevelId()
		{
			if (!DlcManager.IsAllContentSubscribed(this.required_content) && !string.IsNullOrEmpty(this.missing_content_default))
			{
				return this.missing_content_default;
			}
			return this.nosweat_default_level_id;
		}

		public string[] GetRequiredDlcIds()
		{
			return this.required_content;
		}

		public string[] GetForbiddenDlcIds()
		{
			return null;
		}

		protected string default_level_id;

		protected string nosweat_default_level_id;

		public bool deprecated;
	}
}
