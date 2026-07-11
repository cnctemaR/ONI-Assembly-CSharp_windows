using System;
using FMOD;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformIOS : Platform
	{
		static PlatformIOS()
		{
			Settings.AddPlatformTemplate<PlatformIOS>("0f8eb3f400726694eb47beb1a9f94ce8");
		}

		public override string DisplayName
		{
			get
			{
				return "iOS";
			}
		}

		public override void DeclareUnityMappings(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.IPhonePlayer, this);
		}

		public override void LoadPlugins(global::FMOD.System coreSystem, Action<RESULT, string> reportResult)
		{
			PlatformIOS.StaticLoadPlugins(this, coreSystem, reportResult);
		}

		public static void StaticLoadPlugins(Platform platform, global::FMOD.System coreSystem, Action<RESULT, string> reportResult)
		{
			platform.LoadStaticPlugins(coreSystem, reportResult);
		}
	}
}
