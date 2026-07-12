using System;
using System.Collections.Generic;
using System.IO;
using FMOD;
using UnityEngine;

namespace FMODUnity
{
	public class PlatformPlayInEditor : Platform
	{
		public PlatformPlayInEditor()
		{
			base.Identifier = "playInEditor";
		}

		internal override string DisplayName
		{
			get
			{
				return "Editor";
			}
		}

		internal override void DeclareRuntimePlatforms(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.OSXEditor, this);
			settings.DeclareRuntimePlatform(RuntimePlatform.WindowsEditor, this);
			settings.DeclareRuntimePlatform(RuntimePlatform.LinuxEditor, this);
		}

		internal override bool IsIntrinsic
		{
			get
			{
				return true;
			}
		}

		internal override string GetBankFolder()
		{
			Settings instance = Settings.Instance;
			string text = instance.SourceBankPath;
			if (instance.HasPlatforms)
			{
				text = RuntimeUtils.GetCommonPlatformPath(Path.Combine(text, base.BuildDirectory));
			}
			return text;
		}

		internal override void LoadStaticPlugins(global::FMOD.System coreSystem, Action<RESULT, string> reportResult)
		{
		}

		internal override void InitializeProperties()
		{
			base.InitializeProperties();
			Platform.PropertyAccessors.LiveUpdate.Set(this, TriStateBool.Enabled);
			Platform.PropertyAccessors.Overlay.Set(this, TriStateBool.Enabled);
			Platform.PropertyAccessors.SampleRate.Set(this, 48000);
			Platform.PropertyAccessors.RealChannelCount.Set(this, 256);
			Platform.PropertyAccessors.VirtualChannelCount.Set(this, 1024);
		}

		internal override List<CodecChannelCount> DefaultCodecChannels
		{
			get
			{
				return PlatformPlayInEditor.staticCodecChannels;
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
				channels = 256
			}
		};
	}
}
