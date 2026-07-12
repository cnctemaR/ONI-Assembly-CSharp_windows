using System;
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

		public override string DisplayName
		{
			get
			{
				return "Editor";
			}
		}

		public override void DeclareUnityMappings(Settings settings)
		{
			settings.DeclareRuntimePlatform(RuntimePlatform.OSXEditor, this);
			settings.DeclareRuntimePlatform(RuntimePlatform.WindowsEditor, this);
			settings.DeclareRuntimePlatform(RuntimePlatform.LinuxEditor, this);
		}

		public override bool IsIntrinsic
		{
			get
			{
				return true;
			}
		}

		public override string GetBankFolder()
		{
			Settings instance = Settings.Instance;
			string text = instance.SourceBankPath;
			if (instance.HasPlatforms)
			{
				text = RuntimeUtils.GetCommonPlatformPath(Path.Combine(text, base.BuildDirectory));
			}
			return text;
		}

		public override void LoadStaticPlugins(global::FMOD.System coreSystem, Action<RESULT, string> reportResult)
		{
		}

		public override void InitializeProperties()
		{
			base.InitializeProperties();
			Platform.PropertyAccessors.LiveUpdate.Set(this, TriStateBool.Enabled);
			Platform.PropertyAccessors.Overlay.Set(this, TriStateBool.Enabled);
			Platform.PropertyAccessors.SampleRate.Set(this, 48000);
			Platform.PropertyAccessors.RealChannelCount.Set(this, 256);
			Platform.PropertyAccessors.VirtualChannelCount.Set(this, 1024);
		}
	}
}
