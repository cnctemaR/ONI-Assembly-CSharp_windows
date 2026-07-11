using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Access to application run-time data.</para>
	/// </summary>
	[NativeHeader("Runtime/Logging/LogSystem.h")]
	[NativeHeader("Runtime/Misc/SystemInfo.h")]
	[NativeHeader("Runtime/File/ApplicationSpecificPersistentDataPath.h")]
	[NativeHeader("Runtime/Input/GetInput.h")]
	[NativeHeader("Runtime/PreloadManager/LoadSceneOperation.h")]
	[NativeHeader("Runtime/Misc/Player.h")]
	[NativeHeader("Runtime/Misc/PlayerSettings.h")]
	[NativeHeader("Runtime/BaseClasses/IsPlaying.h")]
	[NativeHeader("Runtime/Network/NetworkUtility.h")]
	[NativeHeader("Runtime/Application/ApplicationInfo.h")]
	[NativeHeader("Runtime/PreloadManager/PreloadManager.h")]
	[NativeHeader("Runtime/Utilities/Argv.h")]
	[NativeHeader("Runtime/Utilities/URLUtility.h")]
	[NativeHeader("Runtime/Input/InputManager.h")]
	[NativeHeader("Runtime/Export/Application.bindings.h")]
	[NativeHeader("Runtime/Misc/BuildSettings.h")]
	[NativeHeader("Runtime/Application/AdsIdHandler.h")]
	public class Application
	{
		/// <summary>
		///   <para>Quits the player application.</para>
		/// </summary>
		[FreeFunction("GetInputManager().QuitApplication")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Quit();

		/// <summary>
		///   <para>Cancels quitting the application. This is useful for showing a splash screen at the end of a game.</para>
		/// </summary>
		[Obsolete("CancelQuit is deprecated. Use the wantsToQuit event instead.")]
		[FreeFunction("GetInputManager().CancelQuitApplication")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CancelQuit();

		/// <summary>
		///   <para>Unloads the Unity runtime.</para>
		/// </summary>
		[FreeFunction("Application_Bindings::Unload")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Unload();

		/// <summary>
		///   <para>Is some level being loaded? (Read Only) (Obsolete).</para>
		/// </summary>
		[Obsolete("This property is deprecated, please use LoadLevelAsync to detect if a specific scene is currently loading.")]
		public static extern bool isLoadingLevel
		{
			[FreeFunction("GetPreloadManager().IsLoadingOrQueued")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>How far has the download progressed? [0...1].</para>
		/// </summary>
		/// <param name="levelIndex"></param>
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

		/// <summary>
		///   <para>How far has the download progressed? [0...1].</para>
		/// </summary>
		/// <param name="levelName"></param>
		[Obsolete("Streaming was a Unity Web Player feature, and is removed. This function is deprecated and always returns 1.0.")]
		public static float GetStreamProgressForLevel(string levelName)
		{
			return 1f;
		}

		/// <summary>
		///   <para>How many bytes have we downloaded from the main unity web stream (Read Only).</para>
		/// </summary>
		[Obsolete("Streaming was a Unity Web Player feature, and is removed. This property is deprecated and always returns 0.")]
		public static int streamedBytes
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		///   <para>Indicates whether Unity's webplayer security model is enabled.</para>
		/// </summary>
		[Obsolete("Application.webSecurityEnabled is no longer supported, since the Unity Web Player is no longer supported by Unity", true)]
		public static bool webSecurityEnabled
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		///   <para>Can the streamed level be loaded?</para>
		/// </summary>
		/// <param name="levelIndex"></param>
		public static bool CanStreamedLevelBeLoaded(int levelIndex)
		{
			return levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings;
		}

		/// <summary>
		///   <para>Can the streamed level be loaded?</para>
		/// </summary>
		/// <param name="levelName"></param>
		[FreeFunction("Application_Bindings::CanStreamedLevelBeLoaded")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CanStreamedLevelBeLoaded(string levelName);

		/// <summary>
		///   <para>Returns true when in any kind of player is active.(Read Only).</para>
		/// </summary>
		public static extern bool isPlaying
		{
			[FreeFunction("IsWorldPlaying")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Whether the player currently has focus. Read-only.</para>
		/// </summary>
		public static extern bool isFocused
		{
			[FreeFunction("IsPlayerFocused")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the platform the game is running on (Read Only).</para>
		/// </summary>
		public static extern RuntimePlatform platform
		{
			[FreeFunction("systeminfo::GetRuntimePlatform", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns an array of feature tags in use for this build.</para>
		/// </summary>
		[FreeFunction("GetBuildSettings().GetBuildTags")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetBuildTags();

		/// <summary>
		///   <para>Set an array of feature tags for this build.</para>
		/// </summary>
		/// <param name="buildTags"></param>
		[FreeFunction("GetBuildSettings().SetBuildTags")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetBuildTags(string[] buildTags);

		/// <summary>
		///   <para>Returns a GUID for this build (Read Only).</para>
		/// </summary>
		public static extern string buildGUID
		{
			[FreeFunction("Application_Bindings::GetBuildGUID")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Is the current Runtime platform a known mobile platform.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Is the current Runtime platform a known console platform.</para>
		/// </summary>
		public static bool isConsolePlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				return platform == RuntimePlatform.PS4 || platform == RuntimePlatform.XboxOne;
			}
		}

		/// <summary>
		///   <para>Should the player be running when the application is in the background?</para>
		/// </summary>
		public static extern bool runInBackground
		{
			[FreeFunction("GetPlayerSettingsRunInBackground")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("SetPlayerSettingsRunInBackground")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Is Unity activated with the Pro license?</para>
		/// </summary>
		[FreeFunction("GetBuildSettings().GetHasPROVersion")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasProLicense();

		/// <summary>
		///   <para>Returns true when Unity is launched with the -batchmode flag from the command line (Read Only).</para>
		/// </summary>
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

		/// <summary>
		///   <para>Contains the path to the game data folder (Read Only).</para>
		/// </summary>
		public static extern string dataPath
		{
			[FreeFunction("GetAppDataPath")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Contains the path to the StreamingAssets folder (Read Only).</para>
		/// </summary>
		public static extern string streamingAssetsPath
		{
			[FreeFunction("GetStreamingAssetsPath")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Contains the path to a persistent data directory (Read Only).</para>
		/// </summary>
		[SecurityCritical]
		public static extern string persistentDataPath
		{
			[FreeFunction("GetPersistentDataPathApplicationSpecific")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Contains the path to a temporary data / cache directory (Read Only).</para>
		/// </summary>
		public static extern string temporaryCachePath
		{
			[FreeFunction("GetTemporaryCachePathApplicationSpecific")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The URL of the document (what is shown in a browser's address bar) for WebGL (Read Only).</para>
		/// </summary>
		public static extern string absoluteURL
		{
			[FreeFunction("GetPlayerSettings().GetAbsoluteURL")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Execution of a script function in the contained web page.</para>
		/// </summary>
		/// <param name="script">The Javascript function to call.</param>
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

		/// <summary>
		///   <para>The version of the Unity runtime used to play the content.</para>
		/// </summary>
		public static extern string unityVersion
		{
			[FreeFunction("Application_Bindings::GetUnityVersion")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns application version number  (Read Only).</para>
		/// </summary>
		public static extern string version
		{
			[FreeFunction("GetApplicationInfo().GetVersion")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the name of the store or package that installed the application (Read Only).</para>
		/// </summary>
		public static extern string installerName
		{
			[FreeFunction("GetApplicationInfo().GetInstallerName")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns application identifier at runtime. On Apple platforms this is the 'bundleIdentifier' saved in the info.plist file, on Android it's the 'package' from the AndroidManifest.xml. </para>
		/// </summary>
		public static extern string identifier
		{
			[FreeFunction("GetApplicationInfo().GetApplicationIdentifier")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns application install mode (Read Only).</para>
		/// </summary>
		public static extern ApplicationInstallMode installMode
		{
			[FreeFunction("GetApplicationInfo().GetInstallMode")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns application running in sandbox (Read Only).</para>
		/// </summary>
		public static extern ApplicationSandboxType sandboxType
		{
			[FreeFunction("GetApplicationInfo().GetSandboxType")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns application product name (Read Only).</para>
		/// </summary>
		public static extern string productName
		{
			[FreeFunction("GetPlayerSettings().GetProductName")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Return application company name (Read Only).</para>
		/// </summary>
		public static extern string companyName
		{
			[FreeFunction("GetPlayerSettings().GetCompanyName")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>A unique cloud project identifier. It is unique for every project (Read Only).</para>
		/// </summary>
		public static extern string cloudProjectId
		{
			[FreeFunction("GetPlayerSettings().GetCloudProjectId")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("GetAdsIdHandler().RequestAdsIdAsync")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RequestAdvertisingIdentifierAsync(Application.AdvertisingIdentifierCallback delegateMethod);

		/// <summary>
		///   <para>Opens the url in a browser.</para>
		/// </summary>
		/// <param name="url"></param>
		[FreeFunction("OpenURL")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OpenURL(string url);

		[Obsolete("For internal use only")]
		[FreeFunction("Application_Bindings::ForceCrash")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ForceCrash(int mode);

		/// <summary>
		///   <para>Instructs game to try to render at a specified frame rate.</para>
		/// </summary>
		public static extern int targetFrameRate
		{
			[FreeFunction("GetTargetFrameRate")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("SetTargetFrameRate")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The language the user's operating system is running in.</para>
		/// </summary>
		public static extern SystemLanguage systemLanguage
		{
			[FreeFunction("(SystemLanguage)systeminfo::GetSystemLanguage")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("Application_Bindings::SetLogCallbackDefined")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLogCallbackDefined(bool defined);

		/// <summary>
		///   <para>Stack trace logging options. The default value is StackTraceLogType.ScriptOnly.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Get stack trace logging options. The default value is StackTraceLogType.ScriptOnly.</para>
		/// </summary>
		/// <param name="logType"></param>
		[FreeFunction("GetStackTraceLogType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StackTraceLogType GetStackTraceLogType(LogType logType);

		/// <summary>
		///   <para>Set stack trace logging options. The default value is StackTraceLogType.ScriptOnly.</para>
		/// </summary>
		/// <param name="logType"></param>
		/// <param name="stackTraceType"></param>
		[FreeFunction("SetStackTraceLogType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStackTraceLogType(LogType logType, StackTraceLogType stackTraceType);

		/// <summary>
		///   <para>Priority of background loading thread.</para>
		/// </summary>
		public static extern ThreadPriority backgroundLoadingPriority
		{
			[FreeFunction("GetPreloadManager().GetThreadPriority")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("GetPreloadManager().SetThreadPriority")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns the type of Internet reachability currently possible on the device.</para>
		/// </summary>
		public static extern NetworkReachability internetReachability
		{
			[FreeFunction("GetInternetReachability")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns false if application is altered in any way after it was built.</para>
		/// </summary>
		public static extern bool genuine
		{
			[FreeFunction("IsApplicationGenuine")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns true if application integrity can be confirmed.</para>
		/// </summary>
		public static extern bool genuineCheckAvailable
		{
			[FreeFunction("IsApplicationGenuineAvailable")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Request authorization to use the webcam or microphone in the Web Player.</para>
		/// </summary>
		/// <param name="mode"></param>
		[FreeFunction("Application_Bindings::RequestUserAuthorization")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AsyncOperation RequestUserAuthorization(UserAuthorization mode);

		/// <summary>
		///   <para>Check if the user has authorized use of the webcam or microphone in the Web Player.</para>
		/// </summary>
		/// <param name="mode"></param>
		[FreeFunction("Application_Bindings::HasUserAuthorization")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasUserAuthorization(UserAuthorization mode);

		internal static extern bool submitAnalytics
		{
			[FreeFunction("GetPlayerSettings().GetSubmitAnalytics")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Checks whether splash screen is being shown.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Calls a function in the web page that contains the WebGL Player.</para>
		/// </summary>
		/// <param name="functionName">Name of the function to call.</param>
		/// <param name="args">Array of arguments passed in the call.</param>
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

		/// <summary>
		///   <para>Are we running inside the Unity editor? (Read Only)</para>
		/// </summary>
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

		/// <summary>
		///   <para>Captures a screenshot at path filename as a PNG file.</para>
		/// </summary>
		/// <param name="filename">Pathname to save the screenshot file to.</param>
		/// <param name="superSize">Factor by which to increase resolution.</param>
		[Obsolete("Application.CaptureScreenshot is obsolete. Use ScreenCapture.CaptureScreenshot instead (UnityUpgradable) -> [UnityEngine] UnityEngine.ScreenCapture.CaptureScreenshot(*)", true)]
		public static void CaptureScreenshot(string filename, int superSize)
		{
			throw new NotSupportedException("Application.CaptureScreenshot is obsolete. Use ScreenCapture.CaptureScreenshot instead.");
		}

		/// <summary>
		///   <para>Captures a screenshot at path filename as a PNG file.</para>
		/// </summary>
		/// <param name="filename">Pathname to save the screenshot file to.</param>
		/// <param name="superSize">Factor by which to increase resolution.</param>
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

		/// <summary>
		///   <para>The total number of levels available (Read Only).</para>
		/// </summary>
		[Obsolete("Use SceneManager.sceneCountInBuildSettings")]
		public static int levelCount
		{
			get
			{
				return SceneManager.sceneCountInBuildSettings;
			}
		}

		/// <summary>
		///   <para>The level index that was last loaded (Read Only).</para>
		/// </summary>
		[Obsolete("Use SceneManager to determine what scenes have been loaded")]
		public static int loadedLevel
		{
			get
			{
				return SceneManager.GetActiveScene().buildIndex;
			}
		}

		/// <summary>
		///   <para>The name of the level that was last loaded (Read Only).</para>
		/// </summary>
		[Obsolete("Use SceneManager to determine what scenes have been loaded")]
		public static string loadedLevelName
		{
			get
			{
				return SceneManager.GetActiveScene().name;
			}
		}

		/// <summary>
		///   <para>Note: This is now obsolete. Use SceneManager.LoadScene instead.</para>
		/// </summary>
		/// <param name="index">The level to load.</param>
		/// <param name="name">The name of the level to load.</param>
		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevel(int index)
		{
			SceneManager.LoadScene(index, LoadSceneMode.Single);
		}

		/// <summary>
		///   <para>Note: This is now obsolete. Use SceneManager.LoadScene instead.</para>
		/// </summary>
		/// <param name="index">The level to load.</param>
		/// <param name="name">The name of the level to load.</param>
		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevel(string name)
		{
			SceneManager.LoadScene(name, LoadSceneMode.Single);
		}

		/// <summary>
		///   <para>Loads a level additively.</para>
		/// </summary>
		/// <param name="index"></param>
		/// <param name="name"></param>
		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevelAdditive(int index)
		{
			SceneManager.LoadScene(index, LoadSceneMode.Additive);
		}

		/// <summary>
		///   <para>Loads a level additively.</para>
		/// </summary>
		/// <param name="index"></param>
		/// <param name="name"></param>
		[Obsolete("Use SceneManager.LoadScene")]
		public static void LoadLevelAdditive(string name)
		{
			SceneManager.LoadScene(name, LoadSceneMode.Additive);
		}

		/// <summary>
		///   <para>Loads the level asynchronously in the background.</para>
		/// </summary>
		/// <param name="index"></param>
		/// <param name="levelName"></param>
		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAsync(int index)
		{
			return SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
		}

		/// <summary>
		///   <para>Loads the level asynchronously in the background.</para>
		/// </summary>
		/// <param name="index"></param>
		/// <param name="levelName"></param>
		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAsync(string levelName)
		{
			return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
		}

		/// <summary>
		///   <para>Loads the level additively and asynchronously in the background.</para>
		/// </summary>
		/// <param name="index"></param>
		/// <param name="levelName"></param>
		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAdditiveAsync(int index)
		{
			return SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
		}

		/// <summary>
		///   <para>Loads the level additively and asynchronously in the background.</para>
		/// </summary>
		/// <param name="index"></param>
		/// <param name="levelName"></param>
		[Obsolete("Use SceneManager.LoadSceneAsync")]
		public static AsyncOperation LoadLevelAdditiveAsync(string levelName)
		{
			return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
		}

		/// <summary>
		///   <para>Unloads all GameObject associated with the given scene. Note that assets are currently not unloaded, in order to free up asset memory call Resources.UnloadAllUnusedAssets.</para>
		/// </summary>
		/// <param name="index">Index of the scene in the PlayerSettings to unload.</param>
		/// <param name="scenePath">Name of the scene to Unload.</param>
		/// <returns>
		///   <para>Return true if the scene is unloaded.</para>
		/// </returns>
		[Obsolete("Use SceneManager.UnloadScene")]
		public static bool UnloadLevel(int index)
		{
			return SceneManager.UnloadScene(index);
		}

		/// <summary>
		///   <para>Unloads all GameObject associated with the given scene. Note that assets are currently not unloaded, in order to free up asset memory call Resources.UnloadAllUnusedAssets.</para>
		/// </summary>
		/// <param name="index">Index of the scene in the PlayerSettings to unload.</param>
		/// <param name="scenePath">Name of the scene to Unload.</param>
		/// <returns>
		///   <para>Return true if the scene is unloaded.</para>
		/// </returns>
		[Obsolete("Use SceneManager.UnloadScene")]
		public static bool UnloadLevel(string scenePath)
		{
			return SceneManager.UnloadScene(scenePath);
		}

		private static Application.LogCallback s_LogCallbackHandler;

		private static Application.LogCallback s_LogCallbackHandlerThreaded;

		internal static Application.AdvertisingIdentifierCallback OnAdvertisingIdentifierCallback;

		private static volatile Application.LogCallback s_RegisterLogCallbackDeprecated;

		/// <summary>
		///   <para>Delegate method for fetching advertising ID.</para>
		/// </summary>
		/// <param name="advertisingId">Advertising ID.</param>
		/// <param name="trackingEnabled">Indicates whether user has chosen to limit ad tracking.</param>
		/// <param name="errorMsg">Error message.</param>
		public delegate void AdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled, string errorMsg);

		/// <summary>
		///   <para>This is the delegate function when a mobile device notifies of low memory.</para>
		/// </summary>
		public delegate void LowMemoryCallback();

		/// <summary>
		///   <para>Use this delegate type with Application.logMessageReceived or Application.logMessageReceivedThreaded to monitor what gets logged.</para>
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="stackTrace"></param>
		/// <param name="type"></param>
		public delegate void LogCallback(string condition, string stackTrace, LogType type);
	}
}
