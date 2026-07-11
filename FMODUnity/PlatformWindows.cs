using System;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformWindows : Platform
	{
		static PlatformWindows()
		{
			Settings.AddPlatformTemplate<PlatformWindows>("2c5177b11d81d824dbb064f9ac8527da");
		}

		public override string DisplayName
		{
			get
			{
				return "Windows";
			}
		}

		public override void DeclareUnityMappings(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.WindowsPlayer, this);
			settings.DeclareRuntimePlatform(RuntimePlatform.MetroPlayerX86, this);
			settings.DeclareRuntimePlatform(RuntimePlatform.MetroPlayerX64, this);
			settings.DeclareRuntimePlatform(RuntimePlatform.MetroPlayerARM, this);
		}

		public override string GetPluginPath(string pluginName)
		{
			return string.Format("{0}/X86_64/{1}.dll", this.GetPluginBasePath(), pluginName);
		}
	}
}
