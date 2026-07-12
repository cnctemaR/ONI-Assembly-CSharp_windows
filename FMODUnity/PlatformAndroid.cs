using System;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformAndroid : Platform
	{
		static PlatformAndroid()
		{
			Settings.AddPlatformTemplate<PlatformAndroid>("2fea114e74ecf3c4f920e1d5cc1c4c40");
		}

		public override string DisplayName
		{
			get
			{
				return "Android";
			}
		}

		public override void DeclareRuntimePlatforms(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.Android, this);
		}

		public override string GetBankFolder()
		{
			return PlatformAndroid.StaticGetBankFolder();
		}

		public static string StaticGetBankFolder()
		{
			if (!Settings.Instance.AndroidUseOBB)
			{
				return "file:///android_asset";
			}
			return Application.streamingAssetsPath;
		}

		public override string GetPluginPath(string pluginName)
		{
			return PlatformAndroid.StaticGetPluginPath(pluginName);
		}

		public static string StaticGetPluginPath(string pluginName)
		{
			return string.Format("lib{0}.so", pluginName);
		}
	}
}
