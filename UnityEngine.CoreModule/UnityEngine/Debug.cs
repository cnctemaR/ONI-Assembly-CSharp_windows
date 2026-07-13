using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Diagnostics/Validation.h")]
	[NativeHeader("Runtime/Export/Debug/Debug.bindings.h")]
	[NativeHeader("Runtime/Diagnostics/IntegrityCheck.h")]
	public class Debug
	{
		public static ILogger unityLogger
		{
			get
			{
				return Debug.s_Logger;
			}
		}

		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
		{
			bool flag = true;
			Debug.DrawLine(start, end, color, duration, flag);
		}

		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end, Color color)
		{
			bool flag = true;
			float num = 0f;
			Debug.DrawLine(start, end, color, num, flag);
		}

		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end)
		{
			bool flag = true;
			float num = 0f;
			Color white = Color.white;
			Debug.DrawLine(start, end, white, num, flag);
		}

		[FreeFunction("DebugDrawLine", IsThreadSafe = true)]
		public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
		{
			Debug.DrawLine_Injected(ref start, ref end, ref color, duration, depthTest);
		}

		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
		{
			bool flag = true;
			Debug.DrawRay(start, dir, color, duration, flag);
		}

		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color)
		{
			bool flag = true;
			float num = 0f;
			Debug.DrawRay(start, dir, color, num, flag);
		}

		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir)
		{
			bool flag = true;
			float num = 0f;
			Color white = Color.white;
			Debug.DrawRay(start, dir, white, num, flag);
		}

		public static void DrawRay(Vector3 start, Vector3 dir, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
		{
			Debug.DrawLine(start, start + dir, color, duration, depthTest);
		}

		[FreeFunction("PauseEditor")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Break();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DebugBreak();

		[ThreadSafe]
		public unsafe static int ExtractStackTraceNoAlloc(byte* buffer, int bufferMax, string projectFolder)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(projectFolder, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = projectFolder.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = Debug.ExtractStackTraceNoAlloc_Injected(buffer, bufferMax, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		public static void Log(object message)
		{
			Debug.unityLogger.Log(LogType.Log, message);
		}

		public static void Log(object message, Object context)
		{
			Debug.unityLogger.Log(LogType.Log, message, context);
		}

		public static void LogFormat(string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Log, format, args);
		}

		public static void LogFormat(Object context, string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Log, context, format, args);
		}

		public static void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args)
		{
			DebugLogHandler debugLogHandler = Debug.unityLogger.logHandler as DebugLogHandler;
			bool flag = debugLogHandler == null;
			if (flag)
			{
				Debug.unityLogger.LogFormat(logType, context, format, args);
			}
			else
			{
				bool flag2 = Debug.unityLogger.IsLogTypeAllowed(logType);
				if (flag2)
				{
					debugLogHandler.LogFormat(logType, logOptions, context, format, args);
				}
			}
		}

		public static void LogError(object message)
		{
			Debug.unityLogger.Log(LogType.Error, message);
		}

		public static void LogError(object message, Object context)
		{
			Debug.unityLogger.Log(LogType.Error, message, context);
		}

		public static void LogErrorFormat(string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Error, format, args);
		}

		public static void LogErrorFormat(Object context, string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Error, context, format, args);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearDeveloperConsole();

		public static extern bool developerConsoleEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool developerConsoleVisible
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static void LogException(Exception exception)
		{
			Debug.unityLogger.LogException(exception, null);
		}

		public static void LogException(Exception exception, Object context)
		{
			Debug.unityLogger.LogException(exception, context);
		}

		public static void LogWarning(object message)
		{
			Debug.unityLogger.Log(LogType.Warning, message);
		}

		public static void LogWarning(object message, Object context)
		{
			Debug.unityLogger.Log(LogType.Warning, message, context);
		}

		public static void LogWarningFormat(string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Warning, format, args);
		}

		public static void LogWarningFormat(Object context, string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Warning, context, format, args);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, "Assertion failed");
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, Object context)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, "Assertion failed", context);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, object message)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, string message)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, object message, Object context)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, message, context);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, string message, Object context)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, message, context);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertFormat(bool condition, string format, params object[] args)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.LogFormat(LogType.Assert, format, args);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertFormat(bool condition, Object context, string format, params object[] args)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.LogFormat(LogType.Assert, context, format, args);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertion(object message)
		{
			Debug.unityLogger.Log(LogType.Assert, message);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertion(object message, Object context)
		{
			Debug.unityLogger.Log(LogType.Assert, message, context);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertionFormat(string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Assert, format, args);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertionFormat(Object context, string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Assert, context, format, args);
		}

		public static extern bool isDebugBuild
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeThrows]
		internal static extern DiagnosticSwitch[] diagnosticSwitches
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.TextCoreTextEngineModule", "UnityEngine.IMGUIModule" })]
		internal static DiagnosticSwitch GetDiagnosticSwitch(string name)
		{
			foreach (DiagnosticSwitch diagnosticSwitch in Debug.diagnosticSwitches)
			{
				bool flag = diagnosticSwitch.name == name;
				if (flag)
				{
					return diagnosticSwitch;
				}
			}
			throw new ArgumentException("Could not find DiagnosticSwitch named " + name);
		}

		[RequiredByNativeCode]
		internal static bool CallOverridenDebugHandler(Exception exception, Object obj)
		{
			bool flag = Debug.unityLogger.logHandler is DebugLogHandler;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				try
				{
					Debug.unityLogger.LogException(exception, obj);
				}
				catch (Exception ex)
				{
					Debug.s_DefaultLogger.LogError(string.Format("Invalid exception thrown from custom {0}.LogException(). Message: {1}", Debug.unityLogger.logHandler.GetType(), ex), obj);
					return false;
				}
				flag2 = true;
			}
			return flag2;
		}

		[RequiredByNativeCode]
		internal static bool IsLoggingEnabled()
		{
			bool flag = Debug.unityLogger.logHandler is DebugLogHandler;
			bool flag2;
			if (flag)
			{
				flag2 = Debug.unityLogger.logEnabled;
			}
			else
			{
				flag2 = Debug.s_DefaultLogger.logEnabled;
			}
			return flag2;
		}

		[FreeFunction("RetrieveStartupLogs_Internal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Debug.StartupLog[] RetrieveStartupLogs();

		[FreeFunction("CheckApplicationIntegrity")]
		public static string CheckIntegrity(IntegrityCheckLevel level)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				Debug.CheckIntegrity_Injected(level, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction("IsValidationLevelEnabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsValidationLevelEnabled(ValidationLevel level);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Conditional("UNITY_ASSERTIONS")]
		[Obsolete("Assert(bool, string, params object[]) is obsolete. Use AssertFormat(bool, string, params object[]) (UnityUpgradable) -> AssertFormat(*)", true)]
		public static void Assert(bool condition, string format, params object[] args)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.LogFormat(LogType.Assert, format, args);
			}
		}

		[Obsolete("Debug.logger is obsolete. Please use Debug.unityLogger instead (UnityUpgradable) -> unityLogger")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ILogger logger
		{
			get
			{
				return Debug.s_Logger;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawLine_Injected([In] ref Vector3 start, [In] ref Vector3 end, [DefaultValue("Color.white")] [In] ref Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int ExtractStackTraceNoAlloc_Injected(byte* buffer, int bufferMax, ref ManagedSpanWrapper projectFolder);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CheckIntegrity_Injected(IntegrityCheckLevel level, out ManagedSpanWrapper ret);

		internal static readonly ILogger s_DefaultLogger = new Logger(new DebugLogHandler());

		internal static ILogger s_Logger = new Logger(new DebugLogHandler());

		[NativeHeader("Runtime/Export/Debug/LogCapture.bindings.h")]
		public struct StartupLog
		{
			public long timestamp;

			public LogType logType;

			public string message;
		}
	}
}
