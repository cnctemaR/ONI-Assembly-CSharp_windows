using System;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;

namespace UnityEngine
{
	internal class RuntimeTextSettings : TextSettings
	{
		internal static RuntimeTextSettings defaultTextSettings
		{
			get
			{
				bool flag = RuntimeTextSettings.s_DefaultTextSettings == null;
				if (flag)
				{
					RuntimeTextSettings.s_DefaultTextSettings = ScriptableObject.CreateInstance<RuntimeTextSettings>();
				}
				return RuntimeTextSettings.s_DefaultTextSettings;
			}
		}

		internal override List<FontAsset> GetStaticFallbackOSFontAsset()
		{
			return RuntimeTextSettings.s_FallbackOSFontAssetIMGUIInternal;
		}

		internal override void SetStaticFallbackOSFontAsset(List<FontAsset> fontAssets)
		{
			RuntimeTextSettings.s_FallbackOSFontAssetIMGUIInternal = fontAssets;
		}

		private static RuntimeTextSettings s_DefaultTextSettings;

		private static List<FontAsset> s_FallbackOSFontAssetIMGUIInternal;
	}
}
