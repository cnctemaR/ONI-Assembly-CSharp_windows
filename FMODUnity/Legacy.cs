using System;
using System.Collections.Generic;

namespace FMODUnity
{
	public static class Legacy
	{
		public static void CopySetting<T, U>(List<T> list, Legacy.Platform fromPlatform, Legacy.Platform toPlatform) where T : Legacy.PlatformSetting<U>, new()
		{
			T t = list.Find((T x) => x.Platform == fromPlatform);
			T t2 = list.Find((T x) => x.Platform == toPlatform);
			if (t != null)
			{
				if (t2 == null)
				{
					T t3 = new T();
					t3.Platform = toPlatform;
					t2 = t3;
					list.Add(t2);
				}
				t2.Value = t.Value;
				return;
			}
			if (t2 != null)
			{
				list.Remove(t2);
			}
		}

		public static void CopySetting(List<Legacy.PlatformBoolSetting> list, Legacy.Platform fromPlatform, Legacy.Platform toPlatform)
		{
			Legacy.CopySetting<Legacy.PlatformBoolSetting, TriStateBool>(list, fromPlatform, toPlatform);
		}

		public static void CopySetting(List<Legacy.PlatformIntSetting> list, Legacy.Platform fromPlatform, Legacy.Platform toPlatform)
		{
			Legacy.CopySetting<Legacy.PlatformIntSetting, int>(list, fromPlatform, toPlatform);
		}

		public static string DisplayName(Legacy.Platform platform)
		{
			switch (platform)
			{
			case Legacy.Platform.Desktop:
				return "Desktop";
			case Legacy.Platform.Mobile:
				return "Mobile";
			case Legacy.Platform.MobileHigh:
				return "High-End Mobile";
			case Legacy.Platform.MobileLow:
				return "Low-End Mobile";
			case Legacy.Platform.Console:
				return "Console";
			case Legacy.Platform.Windows:
				return "Windows";
			case Legacy.Platform.Mac:
				return "OSX";
			case Legacy.Platform.Linux:
				return "Linux";
			case Legacy.Platform.iOS:
				return "iOS";
			case Legacy.Platform.Android:
				return "Android";
			case Legacy.Platform.XboxOne:
				return "XBox One";
			case Legacy.Platform.PS4:
				return "PS4";
			case Legacy.Platform.AppleTV:
				return "Apple TV";
			case Legacy.Platform.UWP:
				return "UWP";
			case Legacy.Platform.Switch:
				return "Switch";
			case Legacy.Platform.WebGL:
				return "WebGL";
			case Legacy.Platform.Stadia:
				return "Stadia";
			}
			return "Unknown";
		}

		public static float SortOrder(Legacy.Platform legacyPlatform)
		{
			switch (legacyPlatform)
			{
			case Legacy.Platform.Desktop:
				return 1f;
			case Legacy.Platform.Mobile:
				return 2f;
			case Legacy.Platform.MobileHigh:
				return 2.1f;
			case Legacy.Platform.MobileLow:
				return 2.2f;
			case Legacy.Platform.Console:
				return 3f;
			case Legacy.Platform.Windows:
				return 1.1f;
			case Legacy.Platform.Mac:
				return 1.2f;
			case Legacy.Platform.Linux:
				return 1.3f;
			case Legacy.Platform.XboxOne:
				return 3.1f;
			case Legacy.Platform.PS4:
				return 3.2f;
			case Legacy.Platform.AppleTV:
				return 2.3f;
			case Legacy.Platform.Switch:
				return 3.3f;
			case Legacy.Platform.Stadia:
				return 3.4f;
			}
			return 0f;
		}

		public static Legacy.Platform Parent(Legacy.Platform platform)
		{
			switch (platform)
			{
			case Legacy.Platform.Desktop:
			case Legacy.Platform.Mobile:
			case Legacy.Platform.Console:
				return Legacy.Platform.Default;
			case Legacy.Platform.MobileHigh:
			case Legacy.Platform.MobileLow:
			case Legacy.Platform.iOS:
			case Legacy.Platform.Android:
			case Legacy.Platform.AppleTV:
				return Legacy.Platform.Mobile;
			case Legacy.Platform.Windows:
			case Legacy.Platform.Mac:
			case Legacy.Platform.Linux:
			case Legacy.Platform.UWP:
			case Legacy.Platform.WebGL:
				return Legacy.Platform.Desktop;
			case Legacy.Platform.XboxOne:
			case Legacy.Platform.PS4:
			case Legacy.Platform.Switch:
			case Legacy.Platform.Stadia:
			case Legacy.Platform.Reserved_1:
			case Legacy.Platform.Reserved_2:
			case Legacy.Platform.Reserved_3:
				return Legacy.Platform.Console;
			}
			return Legacy.Platform.None;
		}

		public static bool IsGroup(Legacy.Platform platform)
		{
			return platform - Legacy.Platform.Desktop <= 1 || platform == Legacy.Platform.Console;
		}

		[Serializable]
		public enum Platform
		{
			None,
			PlayInEditor,
			Default,
			Desktop,
			Mobile,
			MobileHigh,
			MobileLow,
			Console,
			Windows,
			Mac,
			Linux,
			iOS,
			Android,
			Deprecated_1,
			XboxOne,
			PS4,
			Deprecated_2,
			Deprecated_3,
			AppleTV,
			UWP,
			Switch,
			WebGL,
			Stadia,
			Reserved_1,
			Reserved_2,
			Reserved_3,
			Count
		}

		public class PlatformSettingBase
		{
			public Legacy.Platform Platform;
		}

		public class PlatformSetting<T> : Legacy.PlatformSettingBase
		{
			public T Value;
		}

		[Serializable]
		public class PlatformIntSetting : Legacy.PlatformSetting<int>
		{
		}

		[Serializable]
		public class PlatformStringSetting : Legacy.PlatformSetting<string>
		{
		}

		[Serializable]
		public class PlatformBoolSetting : Legacy.PlatformSetting<TriStateBool>
		{
		}
	}
}
