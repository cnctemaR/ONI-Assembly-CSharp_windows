using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/UnityAnalytics/RemoteSettings/RemoteSettings.h")]
	[NativeHeader("UnityAnalyticsScriptingClasses.h")]
	[ExcludeFromDocs]
	[StructLayout(LayoutKind.Sequential)]
	public class RemoteConfigSettings : IDisposable
	{
		private RemoteConfigSettings()
		{
		}

		public RemoteConfigSettings(string configKey)
		{
			this.m_Ptr = RemoteConfigSettings.Internal_Create(this, configKey);
			this.Updated = null;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<bool> Updated;

		~RemoteConfigSettings()
		{
			this.Destroy();
		}

		private void Destroy()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				RemoteConfigSettings.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			this.Destroy();
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr Internal_Create(RemoteConfigSettings rcs, string configKey);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_Destroy(IntPtr ptr);

		[RequiredByNativeCode]
		internal static void RemoteConfigSettingsUpdated(RemoteConfigSettings rcs, bool wasLastUpdatedFromServer)
		{
			Action<bool> updated = rcs.Updated;
			if (updated != null)
			{
				updated(wasLastUpdatedFromServer);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool QueueConfig(string name, object param, int ver = 1, string prefix = "");

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SendDeviceInfoInConfigRequest();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ForceUpdate();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool WasLastUpdatedFromServer();

		[ExcludeFromDocs]
		public int GetInt(string key)
		{
			return this.GetInt(key, 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetInt(string key, [DefaultValue("0")] int defaultValue);

		[ExcludeFromDocs]
		public long GetLong(string key)
		{
			return this.GetLong(key, 0L);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern long GetLong(string key, [DefaultValue("0")] long defaultValue);

		[ExcludeFromDocs]
		public float GetFloat(string key)
		{
			return this.GetFloat(key, 0f);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue);

		[ExcludeFromDocs]
		public string GetString(string key)
		{
			return this.GetString(key, "");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetString(string key, [DefaultValue("\"\"")] string defaultValue);

		[ExcludeFromDocs]
		public bool GetBool(string key)
		{
			return this.GetBool(key, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetBool(string key, [DefaultValue("false")] bool defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasKey(string key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetKeys();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void UseSafeLock();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void ReleaseSafeLock();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetSafeTopMap();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetSafeMap(IntPtr m, string key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long GetSafeNumber(IntPtr m, string key, long defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetSafeFloat(IntPtr m, string key, float defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetSafeBool(IntPtr m, string key, bool defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetSafeStringValue(IntPtr m, string key, string defaultValue);

		[NonSerialized]
		internal IntPtr m_Ptr;
	}
}
