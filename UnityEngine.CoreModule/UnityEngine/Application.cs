using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine.Bindings;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Input/InputManager.h")]
	[NativeHeader("Runtime/Misc/BuildSettings.h")]
	[NativeHeader("Runtime/Misc/Player.h")]
	[NativeHeader("Runtime/Input/GetInput.h")]
	[NativeHeader("Runtime/Application/AdsIdHandler.h")]
	[NativeHeader("Runtime/BaseClasses/IsPlaying.h")]
	[NativeHeader("NativeKernel/Logging/LogSystem.h")]
	[NativeHeader("Runtime/Input/TargetFrameRate.h")]
	[NativeHeader("Runtime/File/ApplicationSpecificPersistentDataPath.h")]
	[NativeHeader("Runtime/Misc/PlayerSettings.h")]
	[NativeHeader("Runtime/Export/Application/Application.bindings.h")]
	[NativeHeader("Runtime/Application/ApplicationInfo.h")]
	[NativeHeader("Runtime/Network/NetworkUtility.h")]
	[NativeHeader("Runtime/Utilities/Argv.h")]
	[NativeHeader("Runtime/PreloadManager/LoadSceneOperation.h")]
	[NativeHeader("Runtime/PreloadManager/PreloadManager.h")]
	[NativeHeader("Runtime/Utilities/URLUtility.h")]
	[NativeHeader("Runtime/Misc/SystemInfo.h")]
	public class Application
	{
		[FreeFunction("GetInputManager().QuitApplication")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Quit(int exitCode);

		public static void Quit()
		{
			Application.Quit(0);
		}

		[Obsolete("CancelQuit is deprecated. Use the wantsToQuit event instead.")]
		[FreeFunction("GetInputManager().CancelQuitApplication")]
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

		[FreeFunction("UpdateMemoryUsage")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SimulateMemoryUsage(ApplicationMemoryUsage usage);

		[Obsolete("Streaming was a Unity Web Player feature, and is removed. This function is deprecated and always returns 1.0 for valid level indices.")]
		public static float GetStreamProgressForLevel(int levelIndex)
		{
			bool flag = levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings;
			float num;
			if (flag)
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
		public unsafe static bool CanStreamedLevelBeLoaded(string levelName)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(levelName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = levelName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = Application.CanStreamedLevelBeLoaded_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public static extern bool isPlaying
		{
			[FreeFunction("IsWorldPlaying")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction]
		public static bool IsPlaying([NotNull] Object obj)
		{
			if (obj == null)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(obj);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			return Application.IsPlaying_Injected(intPtr);
		}

		public static extern bool isFocused
		{
			[FreeFunction("IsPlayerFocused")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("GetBuildSettings().GetBuildTags")]
		[Obsolete("Application.GetBuildTags is no longer supported and will be removed.", false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetBuildTags();

		[Obsolete("Application.SetBuildTags is no longer supported and will be removed.", false)]
		[FreeFunction("GetBuildSettings().SetBuildTags")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetBuildTags(string[] buildTags);

		public static string buildGUID
		{
			[FreeFunction("Application_Bindings::GetBuildGUID")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_buildGUID_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
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

		internal static extern bool isBuildingEditorResources
		{
			[FreeFunction("::IsBuildingEditorResources")]
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
		internal unsafe static bool HasARGV(string name)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = Application.HasARGV_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction("GetFirstValueForARGV")]
		internal unsafe static string GetValueForARGV(string name)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				Application.GetValueForARGV_Injected(ref managedSpanWrapper, out managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpanWrapper2;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper2);
			}
			return stringAndDispose;
		}

		public static string dataPath
		{
			[FreeFunction("GetAppDataPath", IsThreadSafe = true)]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_dataPath_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public static string streamingAssetsPath
		{
			[FreeFunction("GetStreamingAssetsPath", IsThreadSafe = true)]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_streamingAssetsPath_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public static string persistentDataPath
		{
			[FreeFunction("GetPersistentDataPathApplicationSpecific")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_persistentDataPath_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public static string temporaryCachePath
		{
			[FreeFunction("GetTemporaryCachePathApplicationSpecific")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_temporaryCachePath_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public static string absoluteURL
		{
			[FreeFunction("GetPlayerSettings().GetAbsoluteURL")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_absoluteURL_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		[Obsolete("Application.ExternalEval is deprecated. See https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html for alternatives.")]
		public static void ExternalEval(string script)
		{
			bool flag = script.Length > 0 && script[script.Length - 1] != ';';
			if (flag)
			{
				script += ";";
			}
			Application.Internal_ExternalCall(script);
		}

		[FreeFunction("Application_Bindings::ExternalCall")]
		private unsafe static void Internal_ExternalCall(string script)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(script, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = script.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Application.Internal_ExternalCall_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public static string unityVersion
		{
			[FreeFunction("Application_Bindings::GetUnityVersion", IsThreadSafe = true)]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_unityVersion_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		internal static extern int unityVersionVer
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			[FreeFunction("Application_Bindings::GetUnityVersionVer", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern int unityVersionMaj
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			[FreeFunction("Application_Bindings::GetUnityVersionMaj", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern int unityVersionMin
		{
			[FreeFunction("Application_Bindings::GetUnityVersionMin", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static string version
		{
			[FreeFunction("GetApplicationInfo().GetVersion")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_version_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public static string installerName
		{
			[FreeFunction("GetApplicationInfo().GetInstallerName")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_installerName_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public static string identifier
		{
			[FreeFunction("GetApplicationInfo().GetApplicationIdentifier")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_identifier_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
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

		public static string productName
		{
			[FreeFunction("GetPlayerSettings().GetProductName")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_productName_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public static string companyName
		{
			[FreeFunction("GetPlayerSettings().GetCompanyName")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_companyName_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public static string cloudProjectId
		{
			[FreeFunction("GetPlayerSettings().GetCloudProjectId")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_cloudProjectId_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		[FreeFunction("GetAdsIdHandler().RequestAdsIdAsync")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RequestAdvertisingIdentifierAsync(Application.AdvertisingIdentifierCallback delegateMethod);

		[FreeFunction("OpenURL")]
		public unsafe static void OpenURL(string url)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(url, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = url.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Application.OpenURL_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

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

		public static string consoleLogPath
		{
			[FreeFunction("GetConsoleLogPath")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Application.get_consoleLogPath_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
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
		public static AsyncOperation RequestUserAuthorization(UserAuthorization mode)
		{
			IntPtr intPtr = Application.RequestUserAuthorization_Injected(mode);
			return (intPtr == 0) ? null : AsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr);
		}

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

		public static extern RuntimePlatform platform
		{
			[FreeFunction("systeminfo::GetRuntimePlatform", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static bool isMobilePlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				RuntimePlatform runtimePlatform = platform;
				if (runtimePlatform <= RuntimePlatform.Android)
				{
					if (runtimePlatform != RuntimePlatform.IPhonePlayer && runtimePlatform != RuntimePlatform.Android)
					{
						goto IL_003A;
					}
				}
				else
				{
					if (runtimePlatform - RuntimePlatform.MetroPlayerX86 <= 2)
					{
						return SystemInfo.deviceType == DeviceType.Handheld;
					}
					if (runtimePlatform != RuntimePlatform.VisionOS)
					{
						goto IL_003A;
					}
				}
				return true;
				IL_003A:
				return false;
			}
		}

		public static bool isConsolePlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				return platform == RuntimePlatform.GameCoreXboxOne || platform == RuntimePlatform.GameCoreXboxSeries || platform == RuntimePlatform.PS4 || platform == RuntimePlatform.PS5 || platform == RuntimePlatform.Switch || platform == RuntimePlatform.Switch2 || platform == RuntimePlatform.XboxOne;
			}
		}

		public static extern SystemLanguage systemLanguage
		{
			[FreeFunction("(SystemLanguage)systeminfo::GetSystemLanguage")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern NetworkReachability internetReachability
		{
			[FreeFunction("GetInternetReachability")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Application.LowMemoryCallback lowMemory;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Application.MemoryUsageChangedCallback memoryUsageChanged;

		[RequiredByNativeCode]
		internal static void CallLowMemory(ApplicationMemoryUsage usage)
		{
			Application.MemoryUsageChangedCallback memoryUsageChangedCallback = Application.memoryUsageChanged;
			bool flag = memoryUsageChangedCallback != null;
			if (flag)
			{
				ApplicationMemoryUsageChange applicationMemoryUsageChange = new ApplicationMemoryUsageChange(usage);
				memoryUsageChangedCallback(in applicationMemoryUsageChange);
			}
			if (usage > ApplicationMemoryUsage.High)
			{
				if (usage != ApplicationMemoryUsage.Critical)
				{
					throw new Exception(string.Format("Unknown application memory usage: {0}", usage));
				}
				Application.LowMemoryCallback lowMemoryCallback = Application.lowMemory;
				bool flag2 = lowMemoryCallback != null;
				if (flag2)
				{
					lowMemoryCallback();
				}
			}
		}

		[RequiredByNativeCode]
		internal static bool HasLogCallback()
		{
			return Application.s_LogCallbackHandler != null || Application.s_LogCallbackHandlerThreaded != null;
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
				bool flag = logCallback != null;
				if (flag)
				{
					logCallback(logString, stackTrace, type);
				}
			}
			Application.LogCallback logCallback2 = Application.s_LogCallbackHandlerThreaded;
			bool flag2 = logCallback2 != null;
			if (flag2)
			{
				logCallback2(logString, stackTrace, type);
			}
		}

		internal static void InvokeOnAdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled)
		{
			bool flag = Application.OnAdvertisingIdentifierCallback != null;
			if (flag)
			{
				Application.OnAdvertisingIdentifierCallback(advertisingId, trackingEnabled, string.Empty);
			}
		}

		private static string ObjectToJSString(object o)
		{
			bool flag = o == null;
			string text;
			if (flag)
			{
				text = "null";
			}
			else
			{
				bool flag2 = o is string;
				if (flag2)
				{
					string text2 = o.ToString().Replace("\\", "\\\\");
					text2 = text2.Replace("\"", "\\\"");
					text2 = text2.Replace("\n", "\\n");
					text2 = text2.Replace("\r", "\\r");
					text2 = text2.Replace("\0", "");
					text2 = text2.Replace("\u2028", "");
					text2 = text2.Replace("\u2029", "");
					text = "\"" + text2 + "\"";
				}
				else
				{
					bool flag3 = o is int || o is short || o is uint || o is ushort || o is byte;
					if (flag3)
					{
						text = o.ToString();
					}
					else
					{
						bool flag4 = o is float;
						if (flag4)
						{
							NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
							text = ((float)o).ToString(numberFormat);
						}
						else
						{
							bool flag5 = o is double;
							if (flag5)
							{
								NumberFormatInfo numberFormat2 = CultureInfo.InvariantCulture.NumberFormat;
								text = ((double)o).ToString(numberFormat2);
							}
							else
							{
								bool flag6 = o is char;
								if (flag6)
								{
									bool flag7 = (char)o == '"';
									if (flag7)
									{
										text = "\"\\\"\"";
									}
									else
									{
										text = "\"" + o.ToString() + "\"";
									}
								}
								else
								{
									bool flag8 = o is IList;
									if (flag8)
									{
										IList list = (IList)o;
										StringBuilder stringBuilder = new StringBuilder();
										stringBuilder.Append("new Array(");
										int count = list.Count;
										for (int i = 0; i < count; i++)
										{
											bool flag9 = i != 0;
											if (flag9)
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
								}
							}
						}
					}
				}
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
				bool flag = i != 0;
				if (flag)
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

		[Obsolete("Use Object.DontDestroyOnLoad instead")]
		public static void DontDestroyOnLoad(Object o)
		{
			bool flag = o != null;
			if (flag)
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
		public static event Action<string> deepLinkActivated;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Func<bool> wantsToQuit;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action quitting;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action unloading;

		[RequiredByNativeCode]
		private static bool Internal_ApplicationWantsToQuit()
		{
			bool flag = Application.wantsToQuit != null;
			if (flag)
			{
				foreach (Func<bool> func in Application.wantsToQuit.GetInvocationList())
				{
					try
					{
						bool flag2 = !func();
						if (flag2)
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

		public static CancellationToken exitCancellationToken
		{
			get
			{
				return Application.s_currentCancellationTokenSource.Token;
			}
		}

		[RequiredByNativeCode]
		private static void Internal_InitializeExitCancellationToken()
		{
			bool flag = Application.s_currentCancellationTokenSource == null || Application.s_currentCancellationTokenSource.IsCancellationRequested;
			if (flag)
			{
				Application.s_currentCancellationTokenSource = new CancellationTokenSource();
			}
		}

		[RequiredByNativeCode]
		private static void Internal_RaiseExitCancellationToken()
		{
			CancellationTokenSource cancellationTokenSource = Application.s_currentCancellationTokenSource;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
		}

		[RequiredByNativeCode]
		private static void Internal_ApplicationQuit()
		{
			bool flag = Application.quitting != null;
			if (flag)
			{
				Application.quitting();
			}
		}

		[RequiredByNativeCode]
		private static void Internal_ApplicationUnload()
		{
			bool flag = Application.unloading != null;
			if (flag)
			{
				Application.unloading();
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
			bool flag = Application.focusChanged != null;
			if (flag)
			{
				Application.focusChanged(focus);
			}
		}

		[RequiredByNativeCode]
		internal static void InvokeDeepLinkActivated(string url)
		{
			bool flag = Application.deepLinkActivated != null;
			if (flag)
			{
				Application.deepLinkActivated(url);
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
			bool flag = Application.s_RegisterLogCallbackDeprecated != null;
			if (flag)
			{
				Application.logMessageReceived -= Application.s_RegisterLogCallbackDeprecated;
				Application.logMessageReceivedThreaded -= Application.s_RegisterLogCallbackDeprecated;
			}
			Application.s_RegisterLogCallbackDeprecated = handler;
			bool flag2 = handler != null;
			if (flag2)
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

		public static bool isEditor
		{
			get
			{
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CanStreamedLevelBeLoaded_Injected(ref ManagedSpanWrapper levelName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsPlaying_Injected(IntPtr obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_buildGUID_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasARGV_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetValueForARGV_Injected(ref ManagedSpanWrapper name, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_dataPath_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_streamingAssetsPath_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_persistentDataPath_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_temporaryCachePath_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_absoluteURL_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ExternalCall_Injected(ref ManagedSpanWrapper script);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_unityVersion_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_version_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_installerName_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_identifier_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_productName_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_companyName_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_cloudProjectId_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OpenURL_Injected(ref ManagedSpanWrapper url);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_consoleLogPath_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr RequestUserAuthorization_Injected(UserAuthorization mode);

		private static Application.LogCallback s_LogCallbackHandler;

		private static Application.LogCallback s_LogCallbackHandlerThreaded;

		internal static Application.AdvertisingIdentifierCallback OnAdvertisingIdentifierCallback;

		private static CancellationTokenSource s_currentCancellationTokenSource = new CancellationTokenSource();

		private static volatile Application.LogCallback s_RegisterLogCallbackDeprecated;

		public delegate void AdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled, string errorMsg);

		public delegate void LowMemoryCallback();

		public delegate void MemoryUsageChangedCallback(in ApplicationMemoryUsageChange usage);

		public delegate void LogCallback(string condition, string stackTrace, LogType type);
	}
}
