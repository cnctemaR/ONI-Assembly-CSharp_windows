using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using UnityEngine;
using UnityEngine.Serialization;

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
				Settings.Initialize();
				return Settings.instance;
			}
		}

		internal static void Initialize()
		{
			if (Settings.instance == null)
			{
				Settings.isInitializing = true;
				Settings.instance = Resources.Load("FMODStudioSettings") as Settings;
				if (Settings.instance == null)
				{
					RuntimeUtils.DebugLog("[FMOD] Cannot find integration settings, creating default settings");
					Settings.instance = ScriptableObject.CreateInstance<Settings>();
					Settings.instance.name = "FMOD Studio Integration Settings";
					Settings.instance.CurrentVersion = 131619;
					Settings.instance.LastEventReferenceScanVersion = 131619;
				}
				Settings.isInitializing = false;
			}
		}

		internal static bool IsInitialized()
		{
			return !(Settings.instance == null) && !Settings.isInitializing;
		}

		internal static IEditorSettings EditorSettings
		{
			get
			{
				return Settings.editorSettings;
			}
			set
			{
				Settings.editorSettings = value;
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

		internal string TargetPath
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

		internal Platform FindPlatform(string identifier)
		{
			foreach (Platform platform in this.Platforms)
			{
				if (platform.Identifier == identifier)
				{
					return platform;
				}
			}
			return null;
		}

		internal bool PlatformExists(string identifier)
		{
			return this.FindPlatform(identifier) != null;
		}

		internal void AddPlatform(Platform platform)
		{
			if (this.PlatformExists(platform.Identifier))
			{
				throw new ArgumentException(string.Format("Duplicate platform identifier: {0}", platform.Identifier));
			}
			this.Platforms.Add(platform);
		}

		internal void RemovePlatform(string identifier)
		{
			this.Platforms.RemoveAll((Platform p) => p.Identifier == identifier);
		}

		internal void LinkPlatform(Platform platform)
		{
			this.LinkPlatformToParent(platform);
			platform.DeclareRuntimePlatforms(this);
		}

		internal void DeclareRuntimePlatform(RuntimePlatform runtimePlatform, Platform platform)
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
				this.SetPlatformParent(platform, this.FindPlatform(platform.ParentIdentifier));
			}
		}

		internal Platform FindCurrentPlatform()
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
			return this.DefaultPlatform;
		}

		private Settings()
		{
			this.MasterBanks = new List<string>();
			this.Banks = new List<string>();
			this.BanksToLoad = new List<string>();
			this.RealChannelSettings = new List<Legacy.PlatformIntSetting>();
			this.VirtualChannelSettings = new List<Legacy.PlatformIntSetting>();
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

		internal void AddPlatformProperties(Platform platform)
		{
			platform.AffirmProperties();
			this.LinkPlatformToParent(platform);
		}

		public void SetPlatformParent(Platform platform, Platform newParent)
		{
			platform.Parent = newParent;
		}

		internal static void AddPlatformTemplate<T>(string identifier) where T : Platform
		{
			Settings.PlatformTemplates.Add(new Settings.PlatformTemplate
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

		internal void OnEnable()
		{
			if (this.hasLoaded)
			{
				return;
			}
			this.hasLoaded = true;
			this.PopulatePlatformsFromAsset();
			this.DefaultPlatform = this.Platforms.FirstOrDefault<Platform>((Platform platform) => platform is PlatformDefault);
			this.PlayInEditorPlatform = this.Platforms.FirstOrDefault<Platform>((Platform platform) => platform is PlatformPlayInEditor);
			this.Platforms.ForEach(new Action<Platform>(this.LinkPlatform));
		}

		private void PopulatePlatformsFromAsset()
		{
			this.Platforms.Clear();
			foreach (Platform platform in Resources.LoadAll<Platform>("FMODStudioSettings"))
			{
				Platform platform2 = this.FindPlatform(platform.Identifier);
				if (platform2 != null)
				{
					Platform platform3;
					if (platform.Active && !platform2.Active)
					{
						this.RemovePlatform(platform2.Identifier);
						platform3 = platform2;
						platform2 = null;
					}
					else
					{
						platform3 = platform;
					}
					RuntimeUtils.DebugLogWarningFormat("FMOD: Cleaning up duplicate platform: ID  = {0}, name = '{1}', type = {2}", new object[]
					{
						platform3.Identifier,
						platform3.DisplayName,
						platform3.GetType().Name
					});
					global::UnityEngine.Object.DestroyImmediate(platform3, true);
				}
				if (platform2 == null)
				{
					platform.EnsurePropertiesAreValid();
					this.AddPlatform(platform);
				}
			}
		}

		internal const string SettingsAssetName = "FMODStudioSettings";

		private static Settings instance = null;

		private static IEditorSettings editorSettings = null;

		private static bool isInitializing = false;

		[SerializeField]
		public bool HasSourceProject = true;

		[SerializeField]
		public bool HasPlatforms = true;

		[SerializeField]
		private string sourceProjectPath;

		[SerializeField]
		private string sourceBankPath;

		[FormerlySerializedAs("SourceBankPathUnformatted")]
		[SerializeField]
		private string sourceBankPathUnformatted;

		[SerializeField]
		public int BankRefreshCooldown = 5;

		[SerializeField]
		public bool ShowBankRefreshWindow = true;

		internal const int BankRefreshPrompt = -1;

		internal const int BankRefreshManual = -2;

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
		public EventLinkage EventLinkage;

		[SerializeField]
		public DEBUG_FLAGS LoggingLevel = DEBUG_FLAGS.WARNING;

		[SerializeField]
		internal List<Legacy.PlatformIntSetting> SpeakerModeSettings;

		[SerializeField]
		internal List<Legacy.PlatformIntSetting> SampleRateSettings;

		[SerializeField]
		internal List<Legacy.PlatformBoolSetting> LiveUpdateSettings;

		[SerializeField]
		internal List<Legacy.PlatformBoolSetting> OverlaySettings;

		[SerializeField]
		internal List<Legacy.PlatformStringSetting> BankDirectorySettings;

		[SerializeField]
		internal List<Legacy.PlatformIntSetting> VirtualChannelSettings;

		[SerializeField]
		internal List<Legacy.PlatformIntSetting> RealChannelSettings;

		[SerializeField]
		internal List<string> Plugins = new List<string>();

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
		public bool AndroidPatchBuild;

		[SerializeField]
		public MeterChannelOrderingType MeterChannelOrdering;

		[SerializeField]
		public bool StopEventsOutsideMaxDistance;

		[SerializeField]
		internal bool BoltUnitOptionsBuildPending;

		[SerializeField]
		public bool EnableErrorCallback;

		[SerializeField]
		internal Settings.SharedLibraryUpdateStages SharedLibraryUpdateStage;

		[SerializeField]
		internal double SharedLibraryTimeSinceStart;

		[SerializeField]
		internal int CurrentVersion;

		[SerializeField]
		public bool HideSetupWizard;

		[SerializeField]
		internal int LastEventReferenceScanVersion;

		[SerializeField]
		public List<Platform> Platforms = new List<Platform>();

		internal Dictionary<RuntimePlatform, List<Platform>> PlatformForRuntimePlatform = new Dictionary<RuntimePlatform, List<Platform>>();

		[NonSerialized]
		public Platform DefaultPlatform;

		[NonSerialized]
		public Platform PlayInEditorPlatform;

		internal static List<Settings.PlatformTemplate> PlatformTemplates = new List<Settings.PlatformTemplate>();

		[NonSerialized]
		private bool hasLoaded;

		internal enum SharedLibraryUpdateStages
		{
			Start,
			DisableExistingLibraries,
			RestartUnity,
			CopyNewLibraries
		}

		internal struct PlatformTemplate
		{
			public string Identifier;

			public Func<Platform> CreateInstance;
		}
	}
}
