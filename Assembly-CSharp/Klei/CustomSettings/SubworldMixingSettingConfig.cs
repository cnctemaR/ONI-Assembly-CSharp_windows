using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;

namespace Klei.CustomSettings
{
	public class SubworldMixingSettingConfig : MixingSettingConfig
	{
		public override string label
		{
			get
			{
				SubworldMixingSettings cachedSubworldMixingSetting = SettingsCache.GetCachedSubworldMixingSetting(base.worldgenPath);
				StringEntry stringEntry;
				if (!Strings.TryGet(cachedSubworldMixingSetting.name, out stringEntry))
				{
					return cachedSubworldMixingSetting.name;
				}
				return stringEntry;
			}
		}

		public override string tooltip
		{
			get
			{
				SubworldMixingSettings cachedSubworldMixingSetting = SettingsCache.GetCachedSubworldMixingSetting(base.worldgenPath);
				StringEntry stringEntry;
				if (!Strings.TryGet(cachedSubworldMixingSetting.description, out stringEntry))
				{
					return cachedSubworldMixingSetting.description;
				}
				return stringEntry;
			}
		}

		public override Sprite icon
		{
			get
			{
				SubworldMixingSettings cachedSubworldMixingSetting = SettingsCache.GetCachedSubworldMixingSetting(base.worldgenPath);
				Sprite sprite = ((cachedSubworldMixingSetting.icon != null) ? Assets.GetSprite(cachedSubworldMixingSetting.icon) : null);
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
				return SettingsCache.GetCachedSubworldMixingSetting(base.worldgenPath).forbiddenClusterTags;
			}
		}

		public override bool isModded
		{
			get
			{
				return SettingsCache.GetCachedSubworldMixingSetting(base.worldgenPath).isModded;
			}
		}

		public SubworldMixingSettingConfig(string id, string worldgenPath, string[] required_content = null, string dlcIdFrom = null, bool triggers_custom_game = true, long coordinate_range = 5L)
			: base(id, null, null, null, worldgenPath, coordinate_range, false, triggers_custom_game, required_content, "", false)
		{
			this.dlcIdFrom = dlcIdFrom;
			List<SettingLevel> list = new List<SettingLevel>
			{
				new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SUBWORLD_MIXING.LEVELS.DISABLED.NAME, DlcManager.FeatureClusterSpaceEnabled() ? UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SUBWORLD_MIXING.LEVELS.DISABLED.TOOLTIP : UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SUBWORLD_MIXING.LEVELS.DISABLED.TOOLTIP_BASEGAME, 0L, null),
				new SettingLevel("TryMixing", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SUBWORLD_MIXING.LEVELS.TRY_MIXING.NAME, DlcManager.FeatureClusterSpaceEnabled() ? UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SUBWORLD_MIXING.LEVELS.TRY_MIXING.TOOLTIP : UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SUBWORLD_MIXING.LEVELS.TRY_MIXING.TOOLTIP_BASEGAME, 1L, null),
				new SettingLevel("GuranteeMixing", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SUBWORLD_MIXING.LEVELS.GUARANTEE_MIXING.NAME, DlcManager.FeatureClusterSpaceEnabled() ? UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SUBWORLD_MIXING.LEVELS.GUARANTEE_MIXING.TOOLTIP : UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SUBWORLD_MIXING.LEVELS.GUARANTEE_MIXING.TOOLTIP_BASEGAME, 2L, null)
			};
			base.StompLevels(list, "Disabled", "Disabled");
		}

		private const int COORDINATE_RANGE = 5;

		public const string DisabledLevelId = "Disabled";

		public const string TryMixingLevelId = "TryMixing";

		public const string GuaranteeMixingLevelId = "GuranteeMixing";
	}
}
