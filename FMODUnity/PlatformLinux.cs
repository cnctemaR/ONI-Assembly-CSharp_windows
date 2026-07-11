using System;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformLinux : Platform
	{
		static PlatformLinux()
		{
			Settings.AddPlatformTemplate<PlatformLinux>("b7716510a1f36934c87976f3a81dbf3d");
		}

		public override string DisplayName
		{
			get
			{
				return "Linux";
			}
		}

		public override void DeclareUnityMappings(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.LinuxPlayer, this);
		}

		public override string GetPluginPath(string pluginName)
		{
			return string.Format("{0}/lib{1}.so", this.GetPluginBasePath(), pluginName);
		}
	}
}
