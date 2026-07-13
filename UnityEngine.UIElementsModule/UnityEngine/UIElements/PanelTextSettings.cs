using System;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements
{
	[HelpURL("UIE-text-setting-asset")]
	public class PanelTextSettings : TextSettings
	{
		internal static PanelTextSettings defaultPanelTextSettings
		{
			get
			{
				PanelTextSettings.InitializeDefaultPanelTextSettingsIfNull();
				return PanelTextSettings.s_DefaultPanelTextSettings;
			}
		}

		internal static void InitializeDefaultPanelTextSettingsIfNull()
		{
			bool flag = PanelTextSettings.s_DefaultPanelTextSettings == null;
			if (flag)
			{
				PanelTextSettings.s_DefaultPanelTextSettings = ScriptableObject.CreateInstance<PanelTextSettings>();
			}
		}

		private static PanelTextSettings s_DefaultPanelTextSettings;
	}
}
