using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements
{
	public class PanelTextSettings : TextSettings
	{
		internal static PanelTextSettings defaultPanelTextSettings
		{
			get
			{
				bool flag = PanelTextSettings.s_DefaultPanelTextSettings == null;
				if (flag)
				{
					bool flag2 = PanelTextSettings.s_DefaultPanelTextSettings == null;
					if (flag2)
					{
						PanelTextSettings.s_DefaultPanelTextSettings = ScriptableObject.CreateInstance<PanelTextSettings>();
					}
				}
				return PanelTextSettings.s_DefaultPanelTextSettings;
			}
		}

		internal static void UpdateLocalizationFontAsset()
		{
			string text = " - Linux";
			Dictionary<SystemLanguage, string> dictionary = new Dictionary<SystemLanguage, string>
			{
				{
					SystemLanguage.English,
					Path.Combine(UIElementsPackageUtility.EditorResourcesBasePath, "UIPackageResources/FontAssets/DynamicOSFontAssets/Localization/English" + text + ".asset")
				},
				{
					SystemLanguage.Japanese,
					Path.Combine(UIElementsPackageUtility.EditorResourcesBasePath, "UIPackageResources/FontAssets/DynamicOSFontAssets/Localization/Japanese" + text + ".asset")
				},
				{
					SystemLanguage.ChineseSimplified,
					Path.Combine(UIElementsPackageUtility.EditorResourcesBasePath, "UIPackageResources/FontAssets/DynamicOSFontAssets/Localization/ChineseSimplified" + text + ".asset")
				},
				{
					SystemLanguage.ChineseTraditional,
					Path.Combine(UIElementsPackageUtility.EditorResourcesBasePath, "UIPackageResources/FontAssets/DynamicOSFontAssets/Localization/ChineseTraditional" + text + ".asset")
				},
				{
					SystemLanguage.Korean,
					Path.Combine(UIElementsPackageUtility.EditorResourcesBasePath, "UIPackageResources/FontAssets/DynamicOSFontAssets/Localization/Korean" + text + ".asset")
				}
			};
			string text2 = Path.Combine(UIElementsPackageUtility.EditorResourcesBasePath, "UIPackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/GlobalFallback" + text + ".asset");
			FontAsset fontAsset = PanelTextSettings.EditorGUIUtilityLoad(dictionary[PanelTextSettings.GetCurrentLanguage()]) as FontAsset;
			FontAsset fontAsset2 = PanelTextSettings.EditorGUIUtilityLoad(text2) as FontAsset;
			PanelTextSettings.defaultPanelTextSettings.fallbackFontAssets[0] = fontAsset;
			PanelTextSettings.defaultPanelTextSettings.fallbackFontAssets[PanelTextSettings.defaultPanelTextSettings.fallbackFontAssets.Count - 1] = fontAsset2;
		}

		internal FontAsset GetCachedFontAsset(Font font)
		{
			return base.GetCachedFontAssetInternal(font);
		}

		private static PanelTextSettings s_DefaultPanelTextSettings;

		internal static Func<string, Object> EditorGUIUtilityLoad;

		internal static Func<SystemLanguage> GetCurrentLanguage;

		internal static readonly string s_DefaultEditorPanelTextSettingPath = "UIPackageResources/Default Editor Text Settings.asset";
	}
}
