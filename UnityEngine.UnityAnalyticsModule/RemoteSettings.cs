using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("UnityAnalyticsScriptingClasses.h")]
	[NativeHeader("Modules/UnityAnalytics/RemoteSettings/RemoteSettings.h")]
	public static class RemoteSettings
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event RemoteSettings.UpdatedEventHandler Updated;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action BeforeFetchFromServer;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<bool, bool, int> Completed;

		[RequiredByNativeCode]
		internal static void RemoteSettingsUpdated(bool wasLastUpdatedFromServer)
		{
			RemoteSettings.UpdatedEventHandler updated = RemoteSettings.Updated;
			if (updated != null)
			{
				updated();
			}
		}

		[RequiredByNativeCode]
		internal static void RemoteSettingsBeforeFetchFromServer()
		{
			Action beforeFetchFromServer = RemoteSettings.BeforeFetchFromServer;
			if (beforeFetchFromServer != null)
			{
				beforeFetchFromServer();
			}
		}

		[RequiredByNativeCode]
		internal static void RemoteSettingsUpdateCompleted(bool wasLastUpdatedFromServer, bool settingsChanged, int response)
		{
			Action<bool, bool, int> completed = RemoteSettings.Completed;
			if (completed != null)
			{
				completed(wasLastUpdatedFromServer, settingsChanged, response);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Calling CallOnUpdate() is not necessary any more and should be removed. Use RemoteSettingsUpdated instead", true)]
		public static void CallOnUpdate()
		{
			throw new NotSupportedException("Calling CallOnUpdate() is not necessary any more and should be removed.");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ForceUpdate();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool WasLastUpdatedFromServer();

		[ExcludeFromDocs]
		public static int GetInt(string key)
		{
			return RemoteSettings.GetInt(key, 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetInt(string key, [UnityEngine.Internal.DefaultValue("0")] int defaultValue);

		[ExcludeFromDocs]
		public static long GetLong(string key)
		{
			return RemoteSettings.GetLong(key, 0L);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetLong(string key, [UnityEngine.Internal.DefaultValue("0")] long defaultValue);

		[ExcludeFromDocs]
		public static float GetFloat(string key)
		{
			return RemoteSettings.GetFloat(key, 0f);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetFloat(string key, [UnityEngine.Internal.DefaultValue("0.0F")] float defaultValue);

		[ExcludeFromDocs]
		public static string GetString(string key)
		{
			return RemoteSettings.GetString(key, "");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetString(string key, [UnityEngine.Internal.DefaultValue("\"\"")] string defaultValue);

		[ExcludeFromDocs]
		public static bool GetBool(string key)
		{
			return RemoteSettings.GetBool(key, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetBool(string key, [UnityEngine.Internal.DefaultValue("false")] bool defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasKey(string key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetKeys();

		public delegate void UpdatedEventHandler();
	}
}
