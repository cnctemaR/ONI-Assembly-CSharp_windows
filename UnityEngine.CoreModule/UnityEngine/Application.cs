using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Application.bindings.h")]
	[NativeHeader("Runtime/Logging/LogSystem.h")]
	[NativeHeader("Runtime/PreloadManager/PreloadManager.h")]
	[NativeHeader("Runtime/Input/GetInput.h")]
	[NativeHeader("Runtime/Application/ApplicationInfo.h")]
	[NativeHeader("Runtime/Application/AdsIdHandler.h")]
	[NativeHeader("Runtime/File/ApplicationSpecificPersistentDataPath.h")]
	[NativeHeader("Runtime/Network/NetworkUtility.h")]
	[NativeHeader("Runtime/Input/InputManager.h")]
	[NativeHeader("Runtime/Utilities/Argv.h")]
	[NativeHeader("Runtime/BaseClasses/IsPlaying.h")]
	[NativeHeader("Runtime/PreloadManager/LoadSceneOperation.h")]
	[NativeHeader("Runtime/Utilities/URLUtility.h")]
	[NativeHeader("Runtime/Misc/SystemInfo.h")]
	[NativeHeader("Runtime/Misc/PlayerSettings.h")]
	[NativeHeader("Runtime/Misc/Player.h")]
	[NativeHeader("Runtime/Misc/BuildSettings.h")]
	public class Application
	{
		[FreeFunction("GetInputManager().QuitApplication")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Quit(int exitCode);

		public static void Quit()
		{
			Application.Quit(0);
		}

		[FreeFunction("GetInputManager().CancelQuitApplication")]
		[Obsolete("CancelQuit is deprecated. Use the wantsToQuit event instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CancelQuit();

		[FreeFunction("Application_Bindings::Unload")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Unload();

		[Obsolete("This property is deprecated, please use LoadLevelAsync to detect if a specific scene is currently loading.")]
		public static extern bool isLoadingLevel
		{
			[FreeFunction("GetPreloadManager().IsLoadingOrQueued")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Streaming was a Unity Web Player feature, and is removed. This function is deprecated and always returns 1.0 for valid level indices.")]
		public static float GetStreamProgressForLevel(int levelIndex)
		{
			float num;
			if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
			{
				num = 1f;
			}
			else
			{
				num = 0f;
			}
			return num;
		}

		[Obsolete("Streaming was a Unity Web Player feature, and is removed. This function is deprecated and always returns 1.0.")]
		public static float GetStreamProgressForLevel(string levelName)
		{
			return 1f;
		}

		[Obsolete("Streaming was a Unity Web Player feature, and is removed. This property is deprecated and always returns 0.")]
		public static int streamedBytes
		{
			get
			{
				return 0;
			}
		}

		[Obsolete("Application.webSecurityEnabled is no longer supported, since the Unity Web Player is no longer supported by Unity", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool webSecurityEnabled
		{
			get
			{
				return false;
			}
		}

		public static bool CanStreamedLevelBeLoaded(int levelIndex)
		{
			return levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings;
		}

		[FreeFunction("Application_Bindings::CanStreamedLevelBeLoaded")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CanStreamedLevelBeLoaded(string levelName);

		public static extern bool isPlaying
		{
			[FreeFunction("IsWorldPlaying")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsPlaying(Object obj);

		public static extern bool isFocused
		{
			[FreeFunction("IsPlayerFocused")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern RuntimePlatform platform
		{
			[FreeFunction("systeminfo::GetRuntimePlatform", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("GetBuildSettings().GetBuildTags")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetBuildTags();

		[FreeFunction("GetBuildSettings().SetBuildTags")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetBuildTags(string[] buildTags);

		public static extern string buildGUID
		{
			[FreeFunction("Application_Bindings::GetBuildGUID")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static bool isMobilePlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				bool flag;
				switch (platform)
				{
				case RuntimePlatform.MetroPlayerX86:
				case RuntimePlatform.MetroPlayerX64:
				case RuntimePlatform.MetroPlayerARM:
					flag = SystemInfo.deviceType == DeviceType.Handheld;
					break;
				default:
					switch (platform)
					{
					case RuntimePlatform.IPhonePlayer:
					case RuntimePlatform.Android:
						return true;
					}
					flag = false;
					break;
				}
				return flag;
			}
		}

		public static bool isConsolePlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				return platform == RuntimePlatform.PS4 || platform == RuntimePlatform.XboxOne;
			}
		}

		public static extern bool runInBackground
		{
			[FreeFunction("GetPlayerSettingsRunInBackground")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("SetPlayerSettingsRunInBackground")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("GetBuildSettings().GetHasPROVersion")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasProLicense();

		public static extern bool isBatchMode
		{
			[FreeFunction("::IsBatchmode")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern bool isTestRun
		{
			[FreeFunction("::IsTestRun")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern bool isHumanControllingUs
		{
			[FreeFunction("::IsHumanControllingUs")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("HasARGV")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasARGV(string name);

		[FreeFunction("GetFirstValueForARGV")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetValueForARGV(string name);

		public static extern string dataPath
		{
			[FreeFunction("GetAppDataPath")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string streamingAssetsPath
		{
			[FreeFunction("GetStreamingAssetsPath", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[SecurityCritical]
		public static extern string persistentDataPath
		{
			[FreeFunction("GetPersistentDataPathApplicationSpecific")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string temporaryCachePath
		{
			[FreeFunction("GetTemporaryCachePathApplicationSpecific")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string absoluteURL
		{
			[FreeFunction("GetPlayerSettings().GetAbsoluteURL")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Application.ExternalEval is deprecated. See https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html for alternatives.")]
		public static void ExternalEval(string script)
		{
			if (script.Length > 0 && script[script.Length - 1] != ';')
			{
				script += ';';
			}
			Application.Internal_ExternalCall(script);
		}

		[FreeFunction("Application_Bindings::ExternalCall")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ExternalCall(string script);

		public static extern string unityVersion
		{
			[FreeFunction("Application_Bindings::GetUnityVersion")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string version
		{
			[FreeFunction("GetApplicationInfo().GetVersion")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string installerName
		{
			[FreeFunction("GetApplicationInfo().GetInstallerName")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string identifier
		{
			[FreeFunction("GetApplicationInfo().GetApplicationIdentifier")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ApplicationInstallMode installMode
		{
			[FreeFunction("GetApplicationInfo().GetInstallMode")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ApplicationSandboxType sandboxType
		{
			[FreeFunction("GetApplicationInfo().GetSandboxType")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string productName
		{
			[FreeFunction("GetPlayerSettings().GetProductName")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string companyName
		{
			[FreeFunction("GetPlayerSettings().GetCompanyName")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string cloudProjectId
		{
			[FreeFunction("GetPlayerSettings().GetCloudProjectId")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("GetAdsIdHandler().RequestAdsIdAsync")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RequestAdvertisingIdentifierAsync(Application.AdvertisingIdentifierCallback delegateMethod);

		[FreeFunction("OpenURL")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OpenURL(string url);

		[Obsolete("Use UnityEngine.Diagnostics.Utils.ForceCrash")]
		public static void ForceCrash(int mode)
		{
			Utils.ForceCrash((ForcedCrashCategory)mode);
		}

		public static extern int targetFrameRate
		{
			[FreeFunction("GetTargetFrameRate")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("SetTargetFrameRate")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern SystemLanguage systemLanguage
		{
			[FreeFunction("(SystemLanguage)systeminfo::GetSystemLanguage")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("Application_Bindings::SetLogCallbackDefined")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLogCallbackDefined(bool defined);

		[Obsolete("Use SetStackTraceLogType/GetStackTraceLogType instead")]
		public static extern StackTraceLogType stackTraceLogType
		{
			[FreeFunction("Application_Bindings::GetStackTraceLogType")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("Application_Bindings::SetStackTraceLogType")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("GetStackTraceLogType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StackTraceLogType GetStackTraceLogType(LogType logType);

		[FreeFunction("SetStackTraceLogType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStackTraceLogType(LogType logType, StackTraceLogType stackTraceType);

		public static extern string consoleLogPath
		{
			[FreeFunction("GetConsoleLogPath")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ThreadPriority backgroundLoadingPriority
		{
			[FreeFunction("GetPreloadManager().GetThreadPriority")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("GetPreloadManager().SetThreadPriority")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern NetworkReachability internetReachability
		{
			[FreeFunction("GetInternetReachability")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool genuine
		{
			[FreeFunction("IsApplicationGenuine")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool genuineCheckAvailable
		{
			[FreeFunction("IsApplicationGenuineAvailable")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("Application_Bindings::RequestUserAuthorization")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AsyncOperation RequestUserAuthorization(UserAuthorization mode);

		[FreeFunction("Application_Bindings::HasUserAuthorization")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasUserAuthorization(UserAuthorization mode);

		internal static extern bool submitAnalytics
		{
			[FreeFunction("GetPlayerSettings().GetSubmitAnalytics")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("This property is deprecated, please use SplashScreen.isFinished instead")]
		public static bool isShowingSplashScreen
		{
			get
			{
				return !SplashScreen.isFinished;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Application.LowMemoryCallback lowMemory;

		[RequiredByNativeCode]
		private static void CallLowMemory()
		{
			Application.LowMemoryCallback lowMemoryCallback = Application.lowMemory;
			if (lowMemoryCallback != null)
			{
				lowMemoryCallback();
			}
		}

		public static event Application.LogCallback logMessageReceived
		{
			add
			{
				Application.s_LogCallbackHandler = (Application.LogCallback)Delegate.Combine(Application.s_LogCallbackHandler, value);
				Application.SetLogCallbackDefined(true);
			}
			remove
			{
				Application.s_LogCallbackHandler = (Application.LogCallback)Delegate.Remove(Application.s_LogCallbackHandler, value);
			}
		}

		public static event Application.LogCallback logMessageReceivedThreaded
		{
			add
			{
				Application.s_LogCallbackHandlerThreaded = (Application.LogCallback)Delegate.Combine(Application.s_LogCallbackHandlerThreaded, value);
				Application.SetLogCallbackDefined(true);
			}
			remove
			{
				Application.s_LogCallbackHandlerThreaded = (Application.LogCallback)Delegate.Remove(Application.s_LogCallbackHandlerThreaded, value);
			}
		}

		[RequiredByNativeCode]
		private static void CallLogCallback(string logString, string stackTrace, LogType type, bool invokedOnMainThread)
		{
			if (invokedOnMainThread)
			{
				Application.LogCallback logCallback = Application.s_LogCallbackHandler;
				if (logCallback != null)
				{
					logCallback(logString, stackTrace, type);
				}
			}
			Application.LogCallback logCallback2 = Application.s_LogCallbackHandlerThreaded;
			if (logCallback2 != null)
			{
				logCallback2(logString, stackTrace, type);
			}
		}

		internal static void InvokeOnAdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled)
		{
			if (Application.OnAdvertisingIdentifierCallback != null)
			{
				Application.OnAdvertisingIdentifierCallback(advertisingId, trackingEnabled, string.Empty);
			}
		}

		private static string ObjectToJSString(object o)
		{
			string text;
			if (o == null)
			{
				text = "null";
			}
			else if (o is string)
			{
				string text2 = o.ToString().Replace("\\", "\\\\");
				text2 = text2.Replace("\"", "\\\"");
				text2 = text2.Replace("\n", "\\n");
				text2 = text2.Replace("\r", "\\r");
				text2 = text2.Replace("\0", "");
				text2 = text2.Replace("\u2028", "");
				text2 = text2.Replace("\u2029", "");
				text = '"' + text2 + '"';
			}
			else if (o is int || o is short || o is uint || o is ushort || o is byte)
			{
				text = o.ToString();
			}
			else if (o is float)
			{
				NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
				text = ((float)o).ToString(numberFormat);
			}
			else if (o is double)
			{
				NumberFormatInfo numberFormat2 = CultureInfo.InvariantCulture.NumberFormat;
				text = ((double)o).ToString(numberFormat2);
			}
			else if (o is char)
			{
				if ((char)o == '"')
				{
					text = "\"\\\"\"";
				}
				else
				{
					text = '"' + o.ToString() + '"';
				}
			}
			else if (o is IList)
			{
				IList list = (IList)o;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("new Array(");
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(Application.ObjectToJSString(list[i]));
				}
				stringBuilder.Append(")");
				text = stringBuilder.ToString();
			}
			else
			{
				text = Application.ObjectToJSString(o.ToString());
			}
			return text;
		}

		[Obsolete("Application.ExternalCall is deprecated. See https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html for alternatives.")]
		public static void ExternalCall(string functionName, params object[] args)
		{
			Application.Internal_ExternalCall(Application.BuildInvocationForArguments(functionName, args));
		}

		private static string BuildInvocationForArguments(string functionName, params object[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(functionName);
			stringBuilder.Append('(');
			int num = args.Length;
			for (int i = 0; i < num; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(Application.ObjectToJSString(args[i]));
			}
			stringBuilder.Append(')');
			stringBuilder.Append(';');
			return stringBuilder.ToString();
		}

		[Obsolete("use Application.isEditor instead")]
		public static bool isPlayer
		{
			get
			{
				return !Application.isEditor;
			}
		}

		public static bool isEditor
		{
			get
			{
				return false;
			}
		}

		[Obsolete("Use Object.DontDestroyOnLoad instead")]
		public static void DontDestroyOnLoad(Object o)
		{
			if (o != null)
			{
				Object.DontDestroyOnLoad(o);
			}
		}

		[Obsolete("Application.CaptureScreenshot is obsolete. Use ScreenCapture.CaptureScreenshot instead (UnityUpgradable) -> [UnityEngine] UnityEngine.ScreenCapture.CaptureScreenshot(*)", true)]
		public static void CaptureScreenshot(string filename, int superSize)
		{
			throw new NotSupportedException("Application.CaptureScreenshot is obsolete. Use ScreenCapture.CaptureScreenshot instead.");
		}

		[Obsolete("Application.CaptureScreenshot is obsolete. Use ScreenCapture.CaptureScreenshot instead (UnityUpgradable) -> [UnityEngine] UnityEngine.ScreenCapture.CaptureScreenshot(*)", true)]
		public static void CaptureScreenshot(string filename)
		{
			throw new NotSupportedException("Application.CaptureScreenshot is obsolete. Use ScreenCapture.CaptureScreenshot instead.");
		}

		public static event UnityAction onBeforeRender
		{
			add
			{
				BeforeRenderHelper.RegisterCallback(value);
			}
			remove
			{
				BeforeRenderHelper.UnregisterCallback(value);
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<bool> focusChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Func<bool> wantsToQuit;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action quitting;

		[RequiredByNativeCode]
		private static bool Internal_ApplicationWantsToQuit()
		{
			if (Application.wantsToQuit != null)
			{
				foreach (Func<bool> func in Application.wantsToQuit.GetInvocationList())
				{
					try
					{
						if (!func())
						{
							return false;
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}
			}
			return true;
		}

		[RequiredByNativeCode]
		private static void Internal_ApplicationQuit()
		{
			if (Application.quitting != null)
			{
				Application.quitting();
			}
		}

		[RequiredByNativeCode]
		internal static void InvokeOnBeforeRender()
		{
			BeforeRenderHelper.Invoke();
		}

		[RequiredByNativeCode]
		internal static void InvokeFocusChanged(bool focus)
		{
			if (Application.focusChanged != null)
			{
				Application.focusChanged(focus);
			}
		}

		[Obsolete("Application.RegisterLogCallback is deprecated. Use Application.logMessageReceived instead.")]
		public static void RegisterLogCallback(Application.LogCallback handler)
		{
			Application.RegisterLogCallback(handler, false);
		}

		[Obsolete("Application.RegisterLogCallbackThreaded is deprecated. Use Application.logMessageReceivedThreaded instead.")]
		public static void RegisterLogCallbackThreaded(Application.LogCallback handler)
		{
			Application.RegisterLogCallback(handler, true);
		}

		private static void RegisterLogCallback(Application.LogCallback handler, bool threaded)
		{
			if (Application.s_RegisterLogCallbackDeprecated != null)
			{
				Application.logMessageReceived -= Application.s_RegisterLogCallbackDeprecated;
				Application.logMessageReceivedThreaded -= Application.s_RegisterLogCallbackDeprecated;
			}
			Application.s_RegisterLogCallbackDeprecated = handler;
			if (handler != null)
			{
				if (threaded)
				{
					Application.logMessageReceivedThreaded += handler;
				}
				else
				{
					Application.logMessageReceived += handler;
				}
			}
		}

		[Obsolete("Use SceneManager.sceneCountInBuildSettings")]
		public static int levelCount
		{
			get
			{
				return SceneManager.sceneCountInBuildSettings;
			}
		}

		[Obsolete("Use SceneManager to determine what scenes have been loaded")]
		public static int loadedLevel
		{
			get
			{
				return SceneManager.GetActiveScene().buildIndex;
			}
		}

		[Obsolete("Use SceneManager to determine what scenes have been loaded")]
		public static string loadedLevelName
		{
			get
			{
				return SceneManager.GetActiveScene().name;
			}
		}

		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevel(int index)
		{
			SceneManager.LoadScene(index, LoadSceneMode.Single);
		}

		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevel(string name)
		{
			SceneManager.LoadScene(name, LoadSceneMode.Single);
		}

		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevelAdditive(int index)
		{
			SceneManager.LoadScene(index, LoadSceneMode.Additive);
		}

		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevelAdditive(string name)
		{
			SceneManager.LoadScene(name, LoadSceneMode.Additive);
		}

		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAsync(int index)
		{
			return SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
		}

		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAsync(string levelName)
		{
			return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
		}

		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAdditiveAsync(int index)
		{
			return SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
		}

		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAdditiveAsync(string levelName)
		{
			return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
		}

		[Obsolete("Use SceneManager.UnloadScene")]
		public static bool UnloadLevel(int index)
		{
			return SceneManager.UnloadScene(index);
		}

		[Obsolete("Use SceneManager.UnloadScene")]
		public static bool UnloadLevel(string scenePath)
		{
			return SceneManager.UnloadScene(scenePath);
		}

		private static Application.LogCallback s_LogCallbackHandler;

		private static Application.LogCallback s_LogCallbackHandlerThreaded;

		internal static Application.AdvertisingIdentifierCallback OnAdvertisingIdentifierCallback;

		private static volatile Application.LogCallback s_RegisterLogCallbackDeprecated;

		public delegate void AdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled, string errorMsg);

		public delegate void LowMemoryCallback();

		public delegate void LogCallback(string condition, string stackTrace, LogType type);
	}
}
