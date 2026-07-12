using System;
using System.Collections.Generic;
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

		public override void DeclareRuntimePlatforms(Settings settings)
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

		public override List<CodecChannelCount> DefaultCodecChannels
		{
			get
			{
				return PlatformWindows.staticCodecChannels;
			}
		}

		private static List<CodecChannelCount> staticCodecChannels = new List<CodecChannelCount>
		{
			new CodecChannelCount
			{
				format = CodecType.FADPCM,
				channels = 0
			},
			new CodecChannelCount
			{
				format = CodecType.Vorbis,
				channels = 32
			}
		};
	}
}
