using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FMODUnity
{
	public class Settings : ScriptableObject
	{
		public static Settings Instance
		{
			get
			{
				if (Settings.instance == null)
				{
					Settings.instance = Resources.Load("FMODStudioSettings") as Settings;
					if (Settings.instance == null)
					{
						global::UnityEngine.Debug.Log("FMOD Studio: cannot find integration settings, creating default settings");
						Settings.instance = ScriptableObject.CreateInstance<Settings>();
						Settings.instance.name = "FMOD Studio Integration Settings";
					}
				}
				return Settings.instance;
			}
		}

		public string SourceProjectPath
		{
			get
			{
				if (string.IsNullOrEmpty(this.sourceProjectPath) && !string.IsNullOrEmpty(this.SourceProjectPathUnformatted))
				{
					this.sourceProjectPath = this.GetPlatformSpecificPath(this.SourceProjectPathUnformatted);
				}
				return this.sourceProjectPath;
			}
			set
			{
				this.sourceProjectPath = this.GetPlatformSpecificPath(value);
			}
		}

		public string SourceBankPath
		{
			get
			{
				if (string.IsNullOrEmpty(this.sourceBankPath) && !string.IsNullOrEmpty(this.SourceBankPathUnformatted))
				{
					this.sourceBankPath = this.GetPlatformSpecificPath(this.SourceBankPathUnformatted);
				}
				return this.sourceBankPath;
			}
			set
			{
				this.sourceBankPath = this.GetPlatformSpecificPath(value);
			}
		}

		public static FMODPlatform GetParent(FMODPlatform platform)
		{
			switch (platform)
			{
			case FMODPlatform.Desktop:
			case FMODPlatform.Mobile:
			case FMODPlatform.Console:
				return FMODPlatform.Default;
			case FMODPlatform.MobileHigh:
			case FMODPlatform.MobileLow:
			case FMODPlatform.iOS:
			case FMODPlatform.Android:
			case FMODPlatform.WindowsPhone:
			case FMODPlatform.PSVita:
			case FMODPlatform.AppleTV:
			case FMODPlatform.Switch:
				return FMODPlatform.Mobile;
			case FMODPlatform.Windows:
			case FMODPlatform.Mac:
			case FMODPlatform.Linux:
			case FMODPlatform.UWP:
				return FMODPlatform.Desktop;
			case FMODPlatform.XboxOne:
			case FMODPlatform.PS4:
			case FMODPlatform.WiiU:
				return FMODPlatform.Console;
			}
			return FMODPlatform.None;
		}

		public static bool HasSetting<T>(List<T> list, FMODPlatform platform) where T : PlatformSettingBase
		{
			return list.Exists((T x) => x.Platform == platform);
		}

		public static U GetSetting<T, U>(List<T> list, FMODPlatform platform, U def) where T : PlatformSetting<U>
		{
			T t = list.Find((T x) => x.Platform == platform);
			if (t != null)
			{
				return t.Value;
			}
			FMODPlatform parent = Settings.GetParent(platform);
			if (parent != FMODPlatform.None)
			{
				return Settings.GetSetting<T, U>(list, parent, def);
			}
			return def;
		}

		public static void SetSetting<T, U>(List<T> list, FMODPlatform platform, U value) where T : PlatformSetting<U>, new()
		{
			T t = list.Find((T x) => x.Platform == platform);
			if (t == null)
			{
				t = new T();
				t.Platform = platform;
				list.Add(t);
			}
			t.Value = value;
		}

		public static void RemoveSetting<T>(List<T> list, FMODPlatform platform) where T : PlatformSettingBase
		{
			list.RemoveAll((T x) => x.Platform == platform);
		}

		public bool IsLiveUpdateEnabled(FMODPlatform platform)
		{
			return Settings.GetSetting<PlatformBoolSetting, TriStateBool>(this.LiveUpdateSettings, platform, TriStateBool.Disabled) == TriStateBool.Enabled;
		}

		public bool IsOverlayEnabled(FMODPlatform platform)
		{
			return Settings.GetSetting<PlatformBoolSetting, TriStateBool>(this.OverlaySettings, platform, TriStateBool.Disabled) == TriStateBool.Enabled;
		}

		public int GetRealChannels(FMODPlatform platform)
		{
			return Settings.GetSetting<PlatformIntSetting, int>(this.RealChannelSettings, platform, 64);
		}

		public int GetVirtualChannels(FMODPlatform platform)
		{
			return Settings.GetSetting<PlatformIntSetting, int>(this.VirtualChannelSettings, platform, 128);
		}

		public int GetSpeakerMode(FMODPlatform platform)
		{
			return Settings.GetSetting<PlatformIntSetting, int>(this.SpeakerModeSettings, platform, 3);
		}

		public int GetSampleRate(FMODPlatform platform)
		{
			return Settings.GetSetting<PlatformIntSetting, int>(this.SampleRateSettings, platform, 48000);
		}

		public string GetBankPlatform(FMODPlatform platform)
		{
			if (!this.HasPlatforms)
			{
				return "";
			}
			return Settings.GetSetting<PlatformStringSetting, string>(this.BankDirectorySettings, platform, "Desktop");
		}

		private Settings()
		{
			this.Banks = new List<string>();
			this.RealChannelSettings = new List<PlatformIntSetting>();
			this.VirtualChannelSettings = new List<PlatformIntSetting>();
			this.LoggingSettings = new List<PlatformBoolSetting>();
			this.LiveUpdateSettings = new List<PlatformBoolSetting>();
			this.OverlaySettings = new List<PlatformBoolSetting>();
			this.SampleRateSettings = new List<PlatformIntSetting>();
			this.SpeakerModeSettings = new List<PlatformIntSetting>();
			this.BankDirectorySettings = new List<PlatformStringSetting>();
			Settings.SetSetting<PlatformBoolSetting, TriStateBool>(this.LoggingSettings, FMODPlatform.PlayInEditor, TriStateBool.Enabled);
			Settings.SetSetting<PlatformBoolSetting, TriStateBool>(this.LiveUpdateSettings, FMODPlatform.PlayInEditor, TriStateBool.Enabled);
			Settings.SetSetting<PlatformBoolSetting, TriStateBool>(this.OverlaySettings, FMODPlatform.PlayInEditor, TriStateBool.Enabled);
			Settings.SetSetting<PlatformIntSetting, int>(this.RealChannelSettings, FMODPlatform.PlayInEditor, 256);
			Settings.SetSetting<PlatformIntSetting, int>(this.VirtualChannelSettings, FMODPlatform.PlayInEditor, 1024);
			Settings.SetSetting<PlatformBoolSetting, TriStateBool>(this.LoggingSettings, FMODPlatform.Default, TriStateBool.Disabled);
			Settings.SetSetting<PlatformBoolSetting, TriStateBool>(this.LiveUpdateSettings, FMODPlatform.Default, TriStateBool.Disabled);
			Settings.SetSetting<PlatformBoolSetting, TriStateBool>(this.OverlaySettings, FMODPlatform.Default, TriStateBool.Disabled);
			Settings.SetSetting<PlatformIntSetting, int>(this.RealChannelSettings, FMODPlatform.Default, 32);
			Settings.SetSetting<PlatformIntSetting, int>(this.VirtualChannelSettings, FMODPlatform.Default, 128);
			Settings.SetSetting<PlatformIntSetting, int>(this.SampleRateSettings, FMODPlatform.Default, 0);
			Settings.SetSetting<PlatformIntSetting, int>(this.SpeakerModeSettings, FMODPlatform.Default, 3);
			this.ImportType = ImportType.StreamingAssets;
			this.AutomaticEventLoading = true;
			this.AutomaticSampleLoading = false;
			this.TargetAssetPath = "";
		}

		private string GetPlatformSpecificPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return path;
			}
			if (Path.DirectorySeparatorChar == '/')
			{
				return path.Replace('\\', '/');
			}
			return path.Replace('/', '\\');
		}

		private const string SettingsAssetName = "FMODStudioSettings";

		private static Settings instance;

		[SerializeField]
		public bool HasSourceProject = true;

		[SerializeField]
		public bool HasPlatforms = true;

		[SerializeField]
		private string sourceProjectPath;

		[SerializeField]
		public string SourceProjectPathUnformatted;

		private string sourceBankPath;

		[SerializeField]
		public string SourceBankPathUnformatted;

		[SerializeField]
		public bool AutomaticEventLoading;

		[SerializeField]
		public bool AutomaticSampleLoading;

		[SerializeField]
		public ImportType ImportType;

		[SerializeField]
		public string TargetAssetPath;

		[SerializeField]
		public List<PlatformIntSetting> SpeakerModeSettings;

		[SerializeField]
		public List<PlatformIntSetting> SampleRateSettings;

		[SerializeField]
		public List<PlatformBoolSetting> LiveUpdateSettings;

		[SerializeField]
		public List<PlatformBoolSetting> OverlaySettings;

		[SerializeField]
		public List<PlatformBoolSetting> LoggingSettings;

		[SerializeField]
		public List<PlatformStringSetting> BankDirectorySettings;

		[SerializeField]
		public List<PlatformIntSetting> VirtualChannelSettings;

		[SerializeField]
		public List<PlatformIntSetting> RealChannelSettings;

		[SerializeField]
		public List<string> Plugins = new List<string>();

		[SerializeField]
		public string MasterBank;

		[SerializeField]
		public List<string> Banks;
	}
}
