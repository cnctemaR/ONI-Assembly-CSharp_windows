using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

namespace Klei.CustomSettings
{
	public class WorldMixingSettingConfig : MixingSettingConfig
	{
		public override string label
		{
			get
			{
				WorldMixingSettings cachedWorldMixingSetting = SettingsCache.GetCachedWorldMixingSetting(base.worldgenPath);
				StringEntry stringEntry;
				if (!Strings.TryGet(cachedWorldMixingSetting.name, out stringEntry))
				{
					return cachedWorldMixingSetting.name;
				}
				return stringEntry;
			}
		}

		public override string tooltip
		{
			get
			{
				WorldMixingSettings cachedWorldMixingSetting = SettingsCache.GetCachedWorldMixingSetting(base.worldgenPath);
				StringEntry stringEntry;
				if (!Strings.TryGet(cachedWorldMixingSetting.description, out stringEntry))
				{
					return cachedWorldMixingSetting.description;
				}
				return stringEntry;
			}
		}

		public override Sprite icon
		{
			get
			{
				WorldMixingSettings cachedWorldMixingSetting = SettingsCache.GetCachedWorldMixingSetting(base.worldgenPath);
				Sprite sprite = ((cachedWorldMixingSetting.icon != null) ? ColonyDestinationAsteroidBeltData.GetUISprite(cachedWorldMixingSetting.icon) : null);
				if (sprite == null)
				{
					sprite = Assets.GetSprite(cachedWorldMixingSetting.icon);
				}
				if (sprite == null)
				{
					sprite = Assets.GetSprite("unknown");
				}
				return sprite;
			}
		}

		public override List<string> forbiddenClusterTags
		{
			get
			{
				return SettingsCache.GetCachedWorldMixingSetting(base.worldgenPath).forbiddenClusterTags;
			}
		}

		public override bool isModded
		{
			get
			{
				return SettingsCache.GetCachedWorldMixingSetting(base.worldgenPath).isModded;
			}
		}

		public WorldMixingSettingConfig(string id, string worldgenPath, string[] required_content = null, string dlcIdFrom = null, bool triggers_custom_game = true, long coordinate_range = 5L)
			: base(id, null, null, null, worldgenPath, coordinate_range, false, triggers_custom_game, required_content, "", false)
		{
			this.dlcIdFrom = dlcIdFrom;
			List<SettingLevel> list = new List<SettingLevel>
			{
				new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_MIXING.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_MIXING.LEVELS.DISABLED.TOOLTIP, 0L, null),
				new SettingLevel("TryMixing", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_MIXING.LEVELS.TRY_MIXING.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_MIXING.LEVELS.TRY_MIXING.TOOLTIP, 1L, null),
				new SettingLevel("GuranteeMixing", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_MIXING.LEVELS.GUARANTEE_MIXING.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_MIXING.LEVELS.GUARANTEE_MIXING.TOOLTIP, 2L, null)
			};
			base.StompLevels(list, "Disabled", "Disabled");
		}

		private const int COORDINATE_RANGE = 5;

		public const string DisabledLevelId = "Disabled";

		public const string TryMixingLevelId = "TryMixing";

		public const string GuaranteeMixingLevelId = "GuranteeMixing";
	}
}
