using System;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformLinux : Platform
	{
		static PlatformLinux()
		{
			Settings.AddPlatformTemplate<PlatformLinux>("b7716510a1f36934c87976f3a81dbf3d");
		}

		internal override string DisplayName
		{
			get
			{
				return "Linux";
			}
		}

		internal override void DeclareRuntimePlatforms(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.LinuxPlayer, this);
		}

		internal override string GetPluginPath(string pluginName)
		{
			return string.Format("{0}/lib{1}.so", this.GetPluginBasePath(), pluginName);
		}

		internal override List<CodecChannelCount> DefaultCodecChannels
		{
			get
			{
				return PlatformLinux.staticCodecChannels;
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
