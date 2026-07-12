using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Serialization;

namespace FMODUnity
{
	public abstract class Platform : ScriptableObject
	{
		internal string Identifier
		{
			get
			{
				return this.identifier;
			}
			set
			{
				this.identifier = value;
			}
		}

		internal abstract string DisplayName { get; }

		internal abstract void DeclareRuntimePlatforms(Settings settings);

		internal virtual float Priority
		{
			get
			{
				return 0f;
			}
		}

		internal virtual bool MatchesCurrentEnvironment
		{
			get
			{
				return true;
			}
		}

		internal virtual bool IsIntrinsic
		{
			get
			{
				return false;
			}
		}

		internal virtual void PreSystemCreate(Action<RESULT, string> reportResult)
		{
		}

		internal virtual void PreInitialize(global::FMOD.Studio.System studioSystem)
		{
		}

		internal virtual string GetBankFolder()
		{
			return Application.streamingAssetsPath;
		}

		protected virtual string GetPluginBasePath()
		{
			return string.Format("{0}/Plugins", Application.dataPath);
		}

		internal virtual string GetPluginPath(string pluginName)
		{
			throw new NotImplementedException(string.Format("Plugins are not implemented on platform {0}", this.Identifier));
		}

		internal virtual void LoadPlugins(global::FMOD.System coreSystem, Action<RESULT, string> reportResult)
		{
			this.LoadDynamicPlugins(coreSystem, reportResult);
			this.LoadStaticPlugins(coreSystem, reportResult);
		}

		internal virtual void LoadDynamicPlugins(global::FMOD.System coreSystem, Action<RESULT, string> reportResult)
		{
			List<string> plugins = this.Plugins;
			if (plugins == null)
			{
				return;
			}
			foreach (string text in plugins)
			{
				if (!string.IsNullOrEmpty(text))
				{
					string pluginPath = this.GetPluginPath(text);
					uint num;
					RESULT result = coreSystem.loadPlugin(pluginPath, out num, 0U);
					if (result == RESULT.ERR_FILE_BAD || result == RESULT.ERR_FILE_NOTFOUND)
					{
						string pluginPath2 = this.GetPluginPath(text + "64");
						result = coreSystem.loadPlugin(pluginPath2, out num, 0U);
					}
					reportResult(result, string.Format("Loading plugin '{0}' from '{1}'", text, pluginPath));
				}
			}
		}

		internal virtual void LoadStaticPlugins(global::FMOD.System coreSystem, Action<RESULT, string> reportResult)
		{
			if (this.StaticPlugins.Count > 0)
			{
				RuntimeUtils.DebugLogWarningFormat("FMOD: {0} static plugins specified, but static plugins are only supported on the IL2CPP scripting backend", new object[] { this.StaticPlugins.Count });
			}
		}

		internal void AffirmProperties()
		{
			if (!this.active)
			{
				this.Properties = new Platform.PropertyStorage();
				this.InitializeProperties();
				this.active = true;
			}
		}

		internal void ClearProperties()
		{
			if (this.active)
			{
				this.Properties = new Platform.PropertyStorage();
				this.active = false;
			}
		}

		internal virtual void InitializeProperties()
		{
			if (!this.IsIntrinsic)
			{
				this.ParentIdentifier = "default";
			}
		}

		internal virtual void EnsurePropertiesAreValid()
		{
			if (!this.IsIntrinsic && string.IsNullOrEmpty(this.ParentIdentifier))
			{
				this.ParentIdentifier = "default";
			}
		}

		internal string ParentIdentifier
		{
			get
			{
				return this.parentIdentifier;
			}
			set
			{
				this.parentIdentifier = value;
			}
		}

		internal bool IsLiveUpdateEnabled
		{
			get
			{
				return this.LiveUpdate == TriStateBool.Enabled;
			}
		}

		internal bool IsOverlayEnabled
		{
			get
			{
				return this.Overlay == TriStateBool.Enabled;
			}
		}

		internal bool Active
		{
			get
			{
				return this.active;
			}
		}

		internal bool HasAnyOverriddenProperties
		{
			get
			{
				return this.active && (this.Properties.LiveUpdate.HasValue || this.Properties.LiveUpdatePort.HasValue || this.Properties.Overlay.HasValue || this.Properties.OverlayPosition.HasValue || this.Properties.OverlayFontSize.HasValue || this.Properties.Logging.HasValue || this.Properties.SampleRate.HasValue || this.Properties.BuildDirectory.HasValue || this.Properties.SpeakerMode.HasValue || this.Properties.VirtualChannelCount.HasValue || this.Properties.RealChannelCount.HasValue || this.Properties.DSPBufferLength.HasValue || this.Properties.DSPBufferCount.HasValue || this.Properties.Plugins.HasValue || this.Properties.StaticPlugins.HasValue);
			}
		}

		public TriStateBool LiveUpdate
		{
			get
			{
				return Platform.PropertyAccessors.LiveUpdate.Get(this);
			}
		}

		public int LiveUpdatePort
		{
			get
			{
				return Platform.PropertyAccessors.LiveUpdatePort.Get(this);
			}
		}

		public TriStateBool Overlay
		{
			get
			{
				return Platform.PropertyAccessors.Overlay.Get(this);
			}
		}

		public ScreenPosition OverlayRect
		{
			get
			{
				return Platform.PropertyAccessors.OverlayPosition.Get(this);
			}
		}

		public int OverlayFontSize
		{
			get
			{
				return Platform.PropertyAccessors.OverlayFontSize.Get(this);
			}
		}

		public void SetOverlayFontSize(int size)
		{
			Platform.PropertyAccessors.OverlayFontSize.Set(this, size);
		}

		public TriStateBool Logging
		{
			get
			{
				return Platform.PropertyAccessors.Logging.Get(this);
			}
		}

		public int SampleRate
		{
			get
			{
				return Platform.PropertyAccessors.SampleRate.Get(this);
			}
		}

		public string BuildDirectory
		{
			get
			{
				return Platform.PropertyAccessors.BuildDirectory.Get(this);
			}
		}

		public SPEAKERMODE SpeakerMode
		{
			get
			{
				return Platform.PropertyAccessors.SpeakerMode.Get(this);
			}
		}

		public int VirtualChannelCount
		{
			get
			{
				return Platform.PropertyAccessors.VirtualChannelCount.Get(this);
			}
		}

		public int RealChannelCount
		{
			get
			{
				return Platform.PropertyAccessors.RealChannelCount.Get(this);
			}
		}

		public int DSPBufferLength
		{
			get
			{
				return Platform.PropertyAccessors.DSPBufferLength.Get(this);
			}
		}

		public int DSPBufferCount
		{
			get
			{
				return Platform.PropertyAccessors.DSPBufferCount.Get(this);
			}
		}

		public List<string> Plugins
		{
			get
			{
				return Platform.PropertyAccessors.Plugins.Get(this);
			}
		}

		public List<string> StaticPlugins
		{
			get
			{
				return Platform.PropertyAccessors.StaticPlugins.Get(this);
			}
		}

		public PlatformCallbackHandler CallbackHandler
		{
			get
			{
				return Platform.PropertyAccessors.CallbackHandler.Get(this);
			}
		}

		internal bool InheritsFrom(Platform platform)
		{
			return platform == this || (this.Parent != null && this.Parent.InheritsFrom(platform));
		}

		internal OUTPUTTYPE GetOutputType()
		{
			if (Enum.IsDefined(typeof(OUTPUTTYPE), this.OutputTypeName))
			{
				return (OUTPUTTYPE)Enum.Parse(typeof(OUTPUTTYPE), this.OutputTypeName);
			}
			return OUTPUTTYPE.AUTODETECT;
		}

		internal virtual List<ThreadAffinityGroup> DefaultThreadAffinities
		{
			get
			{
				return Platform.StaticThreadAffinities;
			}
		}

		public IEnumerable<ThreadAffinityGroup> ThreadAffinities
		{
			get
			{
				if (this.threadAffinities.HasValue)
				{
					return this.threadAffinities.Value;
				}
				return this.DefaultThreadAffinities;
			}
		}

		internal Platform.PropertyThreadAffinityList ThreadAffinitiesProperty
		{
			get
			{
				return this.threadAffinities;
			}
		}

		internal virtual List<CodecChannelCount> DefaultCodecChannels
		{
			get
			{
				return Platform.staticCodecChannels;
			}
		}

		internal List<CodecChannelCount> CodecChannels
		{
			get
			{
				if (this.codecChannels.HasValue)
				{
					return this.codecChannels.Value;
				}
				return this.DefaultCodecChannels;
			}
		}

		internal Platform.PropertyCodecChannels CodecChannelsProperty
		{
			get
			{
				return this.codecChannels;
			}
		}

		internal const float DefaultPriority = 0f;

		internal const string RegisterStaticPluginsClassName = "StaticPluginManager";

		internal const string RegisterStaticPluginsFunctionName = "Register";

		[SerializeField]
		private string identifier;

		[SerializeField]
		private string parentIdentifier;

		[SerializeField]
		private bool active;

		[SerializeField]
		protected Platform.PropertyStorage Properties = new Platform.PropertyStorage();

		[SerializeField]
		[FormerlySerializedAs("outputType")]
		internal string OutputTypeName;

		private static List<ThreadAffinityGroup> StaticThreadAffinities = new List<ThreadAffinityGroup>();

		[SerializeField]
		private Platform.PropertyThreadAffinityList threadAffinities = new Platform.PropertyThreadAffinityList();

		[NonSerialized]
		public Platform Parent;

		private static List<CodecChannelCount> staticCodecChannels = new List<CodecChannelCount>
		{
			new CodecChannelCount
			{
				format = CodecType.FADPCM,
				channels = 32
			},
			new CodecChannelCount
			{
				format = CodecType.Vorbis,
				channels = 0
			}
		};

		[SerializeField]
		private Platform.PropertyCodecChannels codecChannels = new Platform.PropertyCodecChannels();

		public class Property<T>
		{
			public T Value;

			public bool HasValue;
		}

		[Serializable]
		public class PropertyBool : Platform.Property<TriStateBool>
		{
		}

		[Serializable]
		public class PropertyScreenPosition : Platform.Property<ScreenPosition>
		{
		}

		[Serializable]
		public class PropertyInt : Platform.Property<int>
		{
		}

		[Serializable]
		public class PropertySpeakerMode : Platform.Property<SPEAKERMODE>
		{
		}

		[Serializable]
		public class PropertyString : Platform.Property<string>
		{
		}

		[Serializable]
		public class PropertyStringList : Platform.Property<List<string>>
		{
		}

		[Serializable]
		public class PropertyCallbackHandler : Platform.Property<PlatformCallbackHandler>
		{
		}

		internal interface PropertyOverrideControl
		{
			bool HasValue(Platform platform);

			void Clear(Platform platform);
		}

		internal struct PropertyAccessor<T> : Platform.PropertyOverrideControl
		{
			public PropertyAccessor(Func<Platform.PropertyStorage, Platform.Property<T>> getter, T defaultValue)
			{
				this.Getter = getter;
				this.DefaultValue = defaultValue;
			}

			public bool HasValue(Platform platform)
			{
				return platform.Active && this.Getter(platform.Properties).HasValue;
			}

			public T Get(Platform platform)
			{
				Platform platform2 = platform;
				while (platform2 != null)
				{
					if (platform2.Active)
					{
						Platform.Property<T> property = this.Getter(platform2.Properties);
						if (property.HasValue)
						{
							return property.Value;
						}
					}
					platform2 = platform2.Parent;
				}
				return this.DefaultValue;
			}

			public void Set(Platform platform, T value)
			{
				Platform.Property<T> property = this.Getter(platform.Properties);
				property.Value = value;
				property.HasValue = true;
			}

			public void Clear(Platform platform)
			{
				this.Getter(platform.Properties).HasValue = false;
			}

			private readonly Func<Platform.PropertyStorage, Platform.Property<T>> Getter;

			private readonly T DefaultValue;
		}

		[Serializable]
		public class PropertyStorage
		{
			public Platform.PropertyBool LiveUpdate = new Platform.PropertyBool();

			public Platform.PropertyInt LiveUpdatePort = new Platform.PropertyInt();

			public Platform.PropertyBool Overlay = new Platform.PropertyBool();

			public Platform.PropertyScreenPosition OverlayPosition = new Platform.PropertyScreenPosition();

			public Platform.PropertyInt OverlayFontSize = new Platform.PropertyInt();

			public Platform.PropertyBool Logging = new Platform.PropertyBool();

			public Platform.PropertyInt SampleRate = new Platform.PropertyInt();

			public Platform.PropertyString BuildDirectory = new Platform.PropertyString();

			public Platform.PropertySpeakerMode SpeakerMode = new Platform.PropertySpeakerMode();

			public Platform.PropertyInt VirtualChannelCount = new Platform.PropertyInt();

			public Platform.PropertyInt RealChannelCount = new Platform.PropertyInt();

			public Platform.PropertyInt DSPBufferLength = new Platform.PropertyInt();

			public Platform.PropertyInt DSPBufferCount = new Platform.PropertyInt();

			public Platform.PropertyStringList Plugins = new Platform.PropertyStringList();

			public Platform.PropertyStringList StaticPlugins = new Platform.PropertyStringList();

			public Platform.PropertyCallbackHandler CallbackHandler = new Platform.PropertyCallbackHandler();
		}

		internal static class PropertyAccessors
		{
			public static readonly Platform.PropertyAccessor<TriStateBool> LiveUpdate = new Platform.PropertyAccessor<TriStateBool>((Platform.PropertyStorage properties) => properties.LiveUpdate, TriStateBool.Disabled);

			public static readonly Platform.PropertyAccessor<int> LiveUpdatePort = new Platform.PropertyAccessor<int>((Platform.PropertyStorage properties) => properties.LiveUpdatePort, 9264);

			public static readonly Platform.PropertyAccessor<TriStateBool> Overlay = new Platform.PropertyAccessor<TriStateBool>((Platform.PropertyStorage properties) => properties.Overlay, TriStateBool.Disabled);

			public static readonly Platform.PropertyAccessor<ScreenPosition> OverlayPosition = new Platform.PropertyAccessor<ScreenPosition>((Platform.PropertyStorage properties) => properties.OverlayPosition, ScreenPosition.TopLeft);

			public static readonly Platform.PropertyAccessor<int> OverlayFontSize = new Platform.PropertyAccessor<int>((Platform.PropertyStorage properties) => properties.OverlayFontSize, 14);

			public static readonly Platform.PropertyAccessor<TriStateBool> Logging = new Platform.PropertyAccessor<TriStateBool>((Platform.PropertyStorage properties) => properties.Logging, TriStateBool.Disabled);

			public static readonly Platform.PropertyAccessor<int> SampleRate = new Platform.PropertyAccessor<int>((Platform.PropertyStorage properties) => properties.SampleRate, 0);

			public static readonly Platform.PropertyAccessor<string> BuildDirectory = new Platform.PropertyAccessor<string>((Platform.PropertyStorage properties) => properties.BuildDirectory, "Desktop");

			public static readonly Platform.PropertyAccessor<SPEAKERMODE> SpeakerMode = new Platform.PropertyAccessor<SPEAKERMODE>((Platform.PropertyStorage properties) => properties.SpeakerMode, SPEAKERMODE.STEREO);

			public static readonly Platform.PropertyAccessor<int> VirtualChannelCount = new Platform.PropertyAccessor<int>((Platform.PropertyStorage properties) => properties.VirtualChannelCount, 128);

			public static readonly Platform.PropertyAccessor<int> RealChannelCount = new Platform.PropertyAccessor<int>((Platform.PropertyStorage properties) => properties.RealChannelCount, 32);

			public static readonly Platform.PropertyAccessor<int> DSPBufferLength = new Platform.PropertyAccessor<int>((Platform.PropertyStorage properties) => properties.DSPBufferLength, 0);

			public static readonly Platform.PropertyAccessor<int> DSPBufferCount = new Platform.PropertyAccessor<int>((Platform.PropertyStorage properties) => properties.DSPBufferCount, 0);

			public static readonly Platform.PropertyAccessor<List<string>> Plugins = new Platform.PropertyAccessor<List<string>>((Platform.PropertyStorage properties) => properties.Plugins, null);

			public static readonly Platform.PropertyAccessor<List<string>> StaticPlugins = new Platform.PropertyAccessor<List<string>>((Platform.PropertyStorage properties) => properties.StaticPlugins, null);

			public static readonly Platform.PropertyAccessor<PlatformCallbackHandler> CallbackHandler = new Platform.PropertyAccessor<PlatformCallbackHandler>((Platform.PropertyStorage properties) => properties.CallbackHandler, null);
		}

		[Serializable]
		public class PropertyThreadAffinityList : Platform.Property<List<ThreadAffinityGroup>>
		{
		}

		[Serializable]
		internal class PropertyCodecChannels : Platform.Property<List<CodecChannelCount>>
		{
		}
	}
}
