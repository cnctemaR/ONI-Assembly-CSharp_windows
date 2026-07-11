using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using UnityEngine;

namespace FMODUnity
{
	public class Settings : ScriptableObject
	{
		public static Settings Instance
		{
			get
			{
				if (Settings.isInitializing)
				{
					return null;
				}
				if (Settings.instance == null)
				{
					Settings.isInitializing = true;
					Settings.instance = Resources.Load("FMODStudioSettings") as Settings;
					if (Settings.instance == null)
					{
						global::UnityEngine.Debug.Log("[FMOD] Cannot find integration settings, creating default settings");
						Settings.instance = ScriptableObject.CreateInstance<Settings>();
						Settings.instance.name = "FMOD Studio Integration Settings";
					}
					Settings.isInitializing = false;
				}
				return Settings.instance;
			}
		}

		public string SourceProjectPath
		{
			get
			{
				return this.sourceProjectPath;
			}
			set
			{
				this.sourceProjectPath = value;
			}
		}

		public string SourceBankPath
		{
			get
			{
				return this.sourceBankPath;
			}
			set
			{
				this.sourceBankPath = value;
			}
		}

		public string TargetPath
		{
			get
			{
				if (this.ImportType == ImportType.AssetBundle)
				{
					if (string.IsNullOrEmpty(this.TargetAssetPath))
					{
						return Application.dataPath;
					}
					return Application.dataPath + "/" + this.TargetAssetPath;
				}
				else
				{
					if (string.IsNullOrEmpty(this.TargetBankFolder))
					{
						return Application.streamingAssetsPath;
					}
					return Application.streamingAssetsPath + "/" + this.TargetBankFolder;
				}
			}
		}

		public string TargetSubFolder
		{
			get
			{
				if (this.ImportType == ImportType.AssetBundle)
				{
					return this.TargetAssetPath;
				}
				return this.TargetBankFolder;
			}
			set
			{
				if (this.ImportType == ImportType.AssetBundle)
				{
					this.TargetAssetPath = value;
					return;
				}
				this.TargetBankFolder = value;
			}
		}

		public Platform FindPlatform(string identifier)
		{
			Platform platform;
			this.Platforms.TryGetValue(identifier, out platform);
			return platform;
		}

		public void ForEachPlatform(Action<Platform> action)
		{
			foreach (Platform platform in this.Platforms.Values)
			{
				action(platform);
			}
		}

		public Platform DefaultPlatform
		{
			get
			{
				return this.defaultPlatform;
			}
		}

		public Platform PlayInEditorPlatform
		{
			get
			{
				return this.playInEditorPlatform;
			}
		}

		private void LinkPlatform(Platform platform)
		{
			this.LinkPlatformToParent(platform);
			platform.DeclareUnityMappings(this);
		}

		public void DeclareRuntimePlatform(RuntimePlatform runtimePlatform, Platform platform)
		{
			List<Platform> list;
			if (!this.PlatformForRuntimePlatform.TryGetValue(runtimePlatform, out list))
			{
				list = new List<Platform>();
				this.PlatformForRuntimePlatform.Add(runtimePlatform, list);
			}
			list.Add(platform);
			list.Sort((Platform a, Platform b) => b.Priority.CompareTo(a.Priority));
		}

		private void LinkPlatformToParent(Platform platform)
		{
			if (!string.IsNullOrEmpty(platform.ParentIdentifier))
			{
				platform.Parent = this.FindPlatform(platform.ParentIdentifier);
			}
		}

		public Platform FindCurrentPlatform()
		{
			List<Platform> list;
			if (this.PlatformForRuntimePlatform.TryGetValue(Application.platform, out list))
			{
				foreach (Platform platform in list)
				{
					if (platform.MatchesCurrentEnvironment)
					{
						return platform;
					}
				}
			}
			return this.defaultPlatform;
		}

		public SPEAKERMODE GetEditorSpeakerMode()
		{
			return this.playInEditorPlatform.SpeakerMode;
		}

		private Settings()
		{
			this.MasterBanks = new List<string>();
			this.Banks = new List<string>();
			this.BanksToLoad = new List<string>();
			this.RealChannelSettings = new List<Legacy.PlatformIntSetting>();
			this.VirtualChannelSettings = new List<Legacy.PlatformIntSetting>();
			this.LoggingSettings = new List<Legacy.PlatformBoolSetting>();
			this.LiveUpdateSettings = new List<Legacy.PlatformBoolSetting>();
			this.OverlaySettings = new List<Legacy.PlatformBoolSetting>();
			this.SampleRateSettings = new List<Legacy.PlatformIntSetting>();
			this.SpeakerModeSettings = new List<Legacy.PlatformIntSetting>();
			this.BankDirectorySettings = new List<Legacy.PlatformStringSetting>();
			this.ImportType = ImportType.StreamingAssets;
			this.AutomaticEventLoading = true;
			this.AutomaticSampleLoading = false;
			this.EnableMemoryTracking = false;
		}

		public void AddPlatformProperties(Platform platform)
		{
			platform.AffirmProperties();
			this.LinkPlatformToParent(platform);
		}

		public static void AddPlatformTemplate<T>(string identifier) where T : Platform
		{
			Settings.platformTemplates.Add(new Settings.PlatformTemplate
			{
				Identifier = identifier,
				CreateInstance = () => Settings.CreatePlatformInstance<T>(identifier)
			});
		}

		private static Platform CreatePlatformInstance<T>(string identifier) where T : Platform
		{
			T t = ScriptableObject.CreateInstance<T>();
			t.InitializeProperties();
			t.Identifier = identifier;
			return t;
		}

		private void OnEnable()
		{
			this.PopulatePlatformsFromAsset();
			this.defaultPlatform = this.Platforms.Values.FirstOrDefault<Platform>((Platform platform) => platform is PlatformDefault);
			this.playInEditorPlatform = this.Platforms.Values.FirstOrDefault<Platform>((Platform platform) => platform is PlatformPlayInEditor);
			this.ForEachPlatform(new Action<Platform>(this.LinkPlatform));
		}

		private void PopulatePlatformsFromAsset()
		{
			foreach (Platform platform in Resources.LoadAll<Platform>("FMODStudioSettings"))
			{
				if (this.FindPlatform(platform.Identifier) == null)
				{
					platform.EnsurePropertiesAreValid();
					this.Platforms.Add(platform.Identifier, platform);
				}
				else
				{
					global::UnityEngine.Debug.LogWarningFormat("Duplicate platform found in {0}: ID  = {1}, name = '{2}', type = {3}", new object[]
					{
						"FMODStudioSettings",
						platform.Identifier,
						platform.DisplayName,
						platform.GetType().Name
					});
				}
			}
		}

		private const string SettingsAssetName = "FMODStudioSettings";

		private static Settings instance = null;

		private static bool isInitializing = false;

		[SerializeField]
		public bool HasSourceProject = true;

		[SerializeField]
		public bool HasPlatforms = true;

		[SerializeField]
		private string sourceProjectPath;

		[SerializeField]
		private string sourceBankPath;

		[SerializeField]
		public string SourceBankPathUnformatted;

		[SerializeField]
		public bool AutomaticEventLoading;

		[SerializeField]
		public BankLoadType BankLoadType;

		[SerializeField]
		public bool AutomaticSampleLoading;

		[SerializeField]
		public string EncryptionKey;

		[SerializeField]
		public ImportType ImportType;

		[SerializeField]
		public string TargetAssetPath = "FMODBanks";

		[SerializeField]
		public string TargetBankFolder = "";

		[SerializeField]
		public DEBUG_FLAGS LoggingLevel = DEBUG_FLAGS.WARNING;

		[SerializeField]
		public List<Legacy.PlatformIntSetting> SpeakerModeSettings;

		[SerializeField]
		public List<Legacy.PlatformIntSetting> SampleRateSettings;

		[SerializeField]
		public List<Legacy.PlatformBoolSetting> LiveUpdateSettings;

		[SerializeField]
		public List<Legacy.PlatformBoolSetting> OverlaySettings;

		[SerializeField]
		public List<Legacy.PlatformBoolSetting> LoggingSettings;

		[SerializeField]
		public List<Legacy.PlatformStringSetting> BankDirectorySettings;

		[SerializeField]
		public List<Legacy.PlatformIntSetting> VirtualChannelSettings;

		[SerializeField]
		public List<Legacy.PlatformIntSetting> RealChannelSettings;

		[SerializeField]
		public List<string> Plugins = new List<string>();

		[SerializeField]
		public List<string> MasterBanks;

		[SerializeField]
		public List<string> Banks;

		[SerializeField]
		public List<string> BanksToLoad;

		[SerializeField]
		public ushort LiveUpdatePort = 9264;

		[SerializeField]
		public bool EnableMemoryTracking;

		[SerializeField]
		public bool AndroidUseOBB;

		[SerializeField]
		public MeterChannelOrderingType MeterChannelOrdering;

		private Dictionary<string, Platform> Platforms = new Dictionary<string, Platform>();

		private Dictionary<RuntimePlatform, List<Platform>> PlatformForRuntimePlatform = new Dictionary<RuntimePlatform, List<Platform>>();

		[NonSerialized]
		private Platform defaultPlatform;

		[NonSerialized]
		private Platform playInEditorPlatform;

		private static List<Settings.PlatformTemplate> platformTemplates = new List<Settings.PlatformTemplate>();

		private struct PlatformTemplate
		{
			public string Identifier;

			public Func<Platform> CreateInstance;
		}
	}
}
