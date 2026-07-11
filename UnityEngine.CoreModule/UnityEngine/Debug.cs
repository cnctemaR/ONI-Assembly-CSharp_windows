using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Debug.bindings.h")]
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

		[FreeFunction("DebugDrawLine")]
		public static void DrawLine(Vector3 start, Vector3 end, [UnityEngine.Internal.DefaultValue("Color.white")] Color color, [UnityEngine.Internal.DefaultValue("0.0f")] float duration, [UnityEngine.Internal.DefaultValue("true")] bool depthTest)
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

		public static void DrawRay(Vector3 start, Vector3 dir, [UnityEngine.Internal.DefaultValue("Color.white")] Color color, [UnityEngine.Internal.DefaultValue("0.0f")] float duration, [UnityEngine.Internal.DefaultValue("true")] bool depthTest)
		{
			Debug.DrawLine(start, start + dir, color, duration, depthTest);
		}

		[FreeFunction("PauseEditor")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Break();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DebugBreak();

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
			if (!condition)
			{
				Debug.unityLogger.Log(LogType.Assert, "Assertion failed");
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, Object context)
		{
			if (!condition)
			{
				Debug.unityLogger.Log(LogType.Assert, "Assertion failed", context);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, object message)
		{
			if (!condition)
			{
				Debug.unityLogger.Log(LogType.Assert, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				Debug.unityLogger.Log(LogType.Assert, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, object message, Object context)
		{
			if (!condition)
			{
				Debug.unityLogger.Log(LogType.Assert, message, context);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, string message, Object context)
		{
			if (!condition)
			{
				Debug.unityLogger.Log(LogType.Assert, message, context);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertFormat(bool condition, string format, params object[] args)
		{
			if (!condition)
			{
				Debug.unityLogger.LogFormat(LogType.Assert, format, args);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertFormat(bool condition, Object context, string format, params object[] args)
		{
			if (!condition)
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

		[StaticAccessor("GetBuildSettings()", StaticAccessorType.Dot)]
		[NativeProperty(TargetType = TargetType.Field)]
		public static extern bool isDebugBuild
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("DeveloperConsole_OpenConsoleFile")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenConsoleFile();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetDiagnosticSwitches(List<DiagnosticSwitch> results);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object GetDiagnosticSwitch(string name);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetDiagnosticSwitch(string name, object value, bool setPersistent);

		[Obsolete("Assert(bool, string, params object[]) is obsolete. Use AssertFormat(bool, string, params object[]) (UnityUpgradable) -> AssertFormat(*)", true)]
		[Conditional("UNITY_ASSERTIONS")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Assert(bool condition, string format, params object[] args)
		{
			if (!condition)
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
		private static extern void DrawLine_Injected(ref Vector3 start, ref Vector3 end, [UnityEngine.Internal.DefaultValue("Color.white")] ref Color color, [UnityEngine.Internal.DefaultValue("0.0f")] float duration, [UnityEngine.Internal.DefaultValue("true")] bool depthTest);

		internal static ILogger s_Logger = new Logger(new DebugLogHandler());
	}
}
