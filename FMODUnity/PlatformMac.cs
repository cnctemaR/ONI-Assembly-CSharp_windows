using System;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformMac : Platform
	{
		static PlatformMac()
		{
			Settings.AddPlatformTemplate<PlatformMac>("52eb9df5db46521439908db3a29a1bbb");
		}

		public override string DisplayName
		{
			get
			{
				return "macOS";
			}
		}

		public override void DeclareUnityMappings(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.OSXPlayer, this);
		}

		public override string GetPluginPath(string pluginName)
		{
			return string.Format("{0}/{1}.bundle", this.GetPluginBasePath(), pluginName);
		}
	}
}
