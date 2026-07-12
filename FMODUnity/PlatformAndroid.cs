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

		internal override string DisplayName
		{
			get
			{
				return "Android";
			}
		}

		internal override void DeclareRuntimePlatforms(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.Android, this);
		}

		internal override string GetBankFolder()
		{
			return PlatformAndroid.StaticGetBankFolder();
		}

		internal static string StaticGetBankFolder()
		{
			if (!Settings.Instance.AndroidUseOBB && !Settings.Instance.AndroidPatchBuild)
			{
				return "file:///android_asset";
			}
			return Application.streamingAssetsPath;
		}

		internal override string GetPluginPath(string pluginName)
		{
			return PlatformAndroid.StaticGetPluginPath(pluginName);
		}

		internal static string StaticGetPluginPath(string pluginName)
		{
			return string.Format("lib{0}.so", pluginName);
		}
	}
}
