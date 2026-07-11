using System;
using FMOD;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformAppleTV : Platform
	{
		static PlatformAppleTV()
		{
			Settings.AddPlatformTemplate<PlatformAppleTV>("e7a046c753c3c3d4aacc91f6597f310d");
		}

		public override string DisplayName
		{
			get
			{
				return "Apple TV";
			}
		}

		public override void DeclareUnityMappings(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.tvOS, this);
		}

		public override void LoadPlugins(global::FMOD.System coreSystem, Action<RESULT, string> reportResult)
		{
			PlatformIOS.StaticLoadPlugins(this, coreSystem, reportResult);
		}
	}
}
