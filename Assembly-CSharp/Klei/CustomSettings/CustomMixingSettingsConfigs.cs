using System;
using STRINGS;

namespace Klei.CustomSettings
{
	public static class CustomMixingSettingsConfigs
	{
		public static SettingConfig DLC2Mixing = new DlcMixingSettingConfig("DLC2_ID", UI.DLC2.NAME, UI.DLC2.MIXING_TOOLTIP, 5L, false, DlcManager.AVAILABLE_DLC_2, "DLC2_ID", "");

		public static SettingConfig CeresAsteroidMixing = new WorldMixingSettingConfig("CeresAsteroidMixing", "dlc2::worldMixing/CeresMixingSettings", DlcManager.AVAILABLE_DLC_2, "DLC2_ID", true, 5L);

		public static SettingConfig IceCavesMixing = new SubworldMixingSettingConfig("IceCavesMixing", "dlc2::subworldMixing/IceCavesMixingSettings", DlcManager.AVAILABLE_DLC_2, "DLC2_ID", true, 5L);

		public static SettingConfig CarrotQuarryMixing = new SubworldMixingSettingConfig("CarrotQuarryMixing", "dlc2::subworldMixing/CarrotQuarryMixingSettings", DlcManager.AVAILABLE_DLC_2, "DLC2_ID", true, 5L);

		public static SettingConfig SugarWoodsMixing = new SubworldMixingSettingConfig("SugarWoodsMixing", "dlc2::subworldMixing/SugarWoodsMixingSettings", DlcManager.AVAILABLE_DLC_2, "DLC2_ID", true, 5L);
	}
}
