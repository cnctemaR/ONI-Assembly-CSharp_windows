using System;
using System.Threading;
using UnityEngine.Events;

namespace UnityEngine.Device
{
	public static class Application
	{
		public static string absoluteURL
		{
			get
			{
				return Application.absoluteURL;
			}
		}

		public static ThreadPriority backgroundLoadingPriority
		{
			get
			{
				return Application.backgroundLoadingPriority;
			}
			set
			{
				Application.backgroundLoadingPriority = value;
			}
		}

		public static string buildGUID
		{
			get
			{
				return Application.buildGUID;
			}
		}

		public static string cloudProjectId
		{
			get
			{
				return Application.cloudProjectId;
			}
		}

		public static string companyName
		{
			get
			{
				return Application.companyName;
			}
		}

		public static string consoleLogPath
		{
			get
			{
				return Application.consoleLogPath;
			}
		}

		public static string dataPath
		{
			get
			{
				return Application.dataPath;
			}
		}

		public static bool genuine
		{
			get
			{
				return Application.genuine;
			}
		}

		public static bool genuineCheckAvailable
		{
			get
			{
				return Application.genuineCheckAvailable;
			}
		}

		public static string identifier
		{
			get
			{
				return Application.identifier;
			}
		}

		public static string installerName
		{
			get
			{
				return Application.installerName;
			}
		}

		public static ApplicationInstallMode installMode
		{
			get
			{
				return Application.installMode;
			}
		}

		public static NetworkReachability internetReachability
		{
			get
			{
				return Application.internetReachability;
			}
		}

		public static bool isBatchMode
		{
			get
			{
				return Application.isBatchMode;
			}
		}

		public static bool isConsolePlatform
		{
			get
			{
				return Application.isConsolePlatform;
			}
		}

		public static bool isEditor
		{
			get
			{
				return Application.isEditor;
			}
		}

		public static bool isFocused
		{
			get
			{
				return Application.isFocused;
			}
		}

		public static bool isMobilePlatform
		{
			get
			{
				return Application.isMobilePlatform;
			}
		}

		public static bool isPlaying
		{
			get
			{
				return Application.isPlaying;
			}
		}

		public static string persistentDataPath
		{
			get
			{
				return Application.persistentDataPath;
			}
		}

		public static RuntimePlatform platform
		{
			get
			{
				return Application.platform;
			}
		}

		public static string productName
		{
			get
			{
				return Application.productName;
			}
		}

		public static bool runInBackground
		{
			get
			{
				return Application.runInBackground;
			}
			set
			{
				Application.runInBackground = value;
			}
		}

		public static ApplicationSandboxType sandboxType
		{
			get
			{
				return Application.sandboxType;
			}
		}

		public static string streamingAssetsPath
		{
			get
			{
				return Application.streamingAssetsPath;
			}
		}

		public static SystemLanguage systemLanguage
		{
			get
			{
				return Application.systemLanguage;
			}
		}

		public static int targetFrameRate
		{
			get
			{
				return Application.targetFrameRate;
			}
			set
			{
				Application.targetFrameRate = value;
			}
		}

		public static string temporaryCachePath
		{
			get
			{
				return Application.temporaryCachePath;
			}
		}

		public static string unityVersion
		{
			get
			{
				return Application.unityVersion;
			}
		}

		public static string version
		{
			get
			{
				return Application.version;
			}
		}

		public static event Action<string> deepLinkActivated
		{
			add
			{
				Application.deepLinkActivated += value;
			}
			remove
			{
				Application.deepLinkActivated -= value;
			}
		}

		public static event Action<bool> focusChanged
		{
			add
			{
				Application.focusChanged += value;
			}
			remove
			{
				Application.focusChanged -= value;
			}
		}

		public static event Application.LogCallback logMessageReceived
		{
			add
			{
				Application.logMessageReceived += value;
			}
			remove
			{
				Application.logMessageReceived -= value;
			}
		}

		public static event Application.LogCallback logMessageReceivedThreaded
		{
			add
			{
				Application.logMessageReceivedThreaded += value;
			}
			remove
			{
				Application.logMessageReceivedThreaded -= value;
			}
		}

		public static event Application.LowMemoryCallback lowMemory
		{
			add
			{
				Application.lowMemory += value;
			}
			remove
			{
				Application.lowMemory -= value;
			}
		}

		public static event Application.MemoryUsageChangedCallback memoryUsageChanged
		{
			add
			{
				Application.memoryUsageChanged += value;
			}
			remove
			{
				Application.memoryUsageChanged -= value;
			}
		}

		public static event UnityAction onBeforeRender
		{
			add
			{
				Application.onBeforeRender += value;
			}
			remove
			{
				Application.onBeforeRender -= value;
			}
		}

		public static event Action quitting
		{
			add
			{
				Application.quitting += value;
			}
			remove
			{
				Application.quitting -= value;
			}
		}

		public static event Func<bool> wantsToQuit
		{
			add
			{
				Application.wantsToQuit += value;
			}
			remove
			{
				Application.wantsToQuit -= value;
			}
		}

		public static event Action unloading
		{
			add
			{
				Application.unloading += value;
			}
			remove
			{
				Application.unloading -= value;
			}
		}

		public static bool CanStreamedLevelBeLoaded(int levelIndex)
		{
			return Application.CanStreamedLevelBeLoaded(levelIndex);
		}

		public static bool CanStreamedLevelBeLoaded(string levelName)
		{
			return Application.CanStreamedLevelBeLoaded(levelName);
		}

		[Obsolete("Application.GetBuildTags is no longer supported and will be removed.", false)]
		public static string[] GetBuildTags()
		{
			return Application.GetBuildTags();
		}

		[Obsolete("Application.SetBuildTags is no longer supported and will be removed.", false)]
		public static void SetBuildTags(string[] buildTags)
		{
			Application.SetBuildTags(buildTags);
		}

		public static StackTraceLogType GetStackTraceLogType(LogType logType)
		{
			return Application.GetStackTraceLogType(logType);
		}

		public static bool HasProLicense()
		{
			return Application.HasProLicense();
		}

		public static bool HasUserAuthorization(UserAuthorization mode)
		{
			return Application.HasUserAuthorization(mode);
		}

		public static bool IsPlaying(Object obj)
		{
			return Application.IsPlaying(obj);
		}

		public static void OpenURL(string url)
		{
			Application.OpenURL(url);
		}

		public static void Quit()
		{
			Application.Quit();
		}

		public static void Quit(int exitCode)
		{
			Application.Quit(exitCode);
		}

		public static bool RequestAdvertisingIdentifierAsync(Application.AdvertisingIdentifierCallback delegateMethod)
		{
			return Application.RequestAdvertisingIdentifierAsync(delegateMethod);
		}

		public static AsyncOperation RequestUserAuthorization(UserAuthorization mode)
		{
			return Application.RequestUserAuthorization(mode);
		}

		public static void SetStackTraceLogType(LogType logType, StackTraceLogType stackTraceType)
		{
			Application.SetStackTraceLogType(logType, stackTraceType);
		}

		public static void Unload()
		{
			Application.Unload();
		}

		public static CancellationToken exitCancellationToken
		{
			get
			{
				return Application.exitCancellationToken;
			}
		}
	}
}
