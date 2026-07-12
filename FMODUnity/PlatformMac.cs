using System;
using System.Collections.Generic;
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

		public override void DeclareRuntimePlatforms(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.OSXPlayer, this);
		}

		public override string GetPluginPath(string pluginName)
		{
			return string.Format("{0}/{1}.bundle", this.GetPluginBasePath(), pluginName);
		}

		public override List<CodecChannelCount> DefaultCodecChannels
		{
			get
			{
				return PlatformMac.staticCodecChannels;
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
