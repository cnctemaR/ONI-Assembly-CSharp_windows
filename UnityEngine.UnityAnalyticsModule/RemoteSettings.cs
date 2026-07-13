using System;
using System.Collections.Generic;
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
			bool flag = updated != null;
			if (flag)
			{
				updated();
			}
		}

		[RequiredByNativeCode]
		internal static void RemoteSettingsBeforeFetchFromServer()
		{
			Action beforeFetchFromServer = RemoteSettings.BeforeFetchFromServer;
			bool flag = beforeFetchFromServer != null;
			if (flag)
			{
				beforeFetchFromServer();
			}
		}

		[RequiredByNativeCode]
		internal static void RemoteSettingsUpdateCompleted(bool wasLastUpdatedFromServer, bool settingsChanged, int response)
		{
			Action<bool, bool, int> completed = RemoteSettings.Completed;
			bool flag = completed != null;
			if (flag)
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

		public unsafe static int GetInt(string key, [DefaultValue("0")] int defaultValue)
		{
			int int_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				int_Injected = RemoteSettings.GetInt_Injected(ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return int_Injected;
		}

		[ExcludeFromDocs]
		public static long GetLong(string key)
		{
			return RemoteSettings.GetLong(key, 0L);
		}

		public unsafe static long GetLong(string key, [DefaultValue("0")] long defaultValue)
		{
			long long_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				long_Injected = RemoteSettings.GetLong_Injected(ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return long_Injected;
		}

		[ExcludeFromDocs]
		public static float GetFloat(string key)
		{
			return RemoteSettings.GetFloat(key, 0f);
		}

		public unsafe static float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue)
		{
			float float_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				float_Injected = RemoteSettings.GetFloat_Injected(ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return float_Injected;
		}

		[ExcludeFromDocs]
		public static string GetString(string key)
		{
			return RemoteSettings.GetString(key, "");
		}

		public unsafe static string GetString(string key, [DefaultValue("\"\"")] string defaultValue)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(defaultValue, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = defaultValue.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper3;
				RemoteSettings.GetString_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, out managedSpanWrapper3);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				ManagedSpanWrapper managedSpanWrapper3;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper3);
			}
			return stringAndDispose;
		}

		[ExcludeFromDocs]
		public static bool GetBool(string key)
		{
			return RemoteSettings.GetBool(key, false);
		}

		public unsafe static bool GetBool(string key, [DefaultValue("false")] bool defaultValue)
		{
			bool bool_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				bool_Injected = RemoteSettings.GetBool_Injected(ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return bool_Injected;
		}

		public unsafe static bool HasKey(string key)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = RemoteSettings.HasKey_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetKeys();

		public static T GetObject<T>(string key = "")
		{
			return (T)((object)RemoteSettings.GetObject(typeof(T), key));
		}

		public static object GetObject(Type type, string key = "")
		{
			bool flag = type == null;
			if (flag)
			{
				throw new ArgumentNullException("type");
			}
			bool flag2 = type.IsAbstract || type.IsSubclassOf(typeof(Object));
			if (flag2)
			{
				throw new ArgumentException("Cannot deserialize to new instances of type '" + type.Name + ".'");
			}
			return RemoteSettings.GetAsScriptingObject(type, null, key);
		}

		public static object GetObject(string key, object defaultValue)
		{
			bool flag = defaultValue == null;
			if (flag)
			{
				throw new ArgumentNullException("defaultValue");
			}
			Type type = defaultValue.GetType();
			bool flag2 = type.IsAbstract || type.IsSubclassOf(typeof(Object));
			if (flag2)
			{
				throw new ArgumentException("Cannot deserialize to new instances of type '" + type.Name + ".'");
			}
			return RemoteSettings.GetAsScriptingObject(type, defaultValue, key);
		}

		internal unsafe static object GetAsScriptingObject(Type t, object defaultValue, string key)
		{
			object asScriptingObject_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				asScriptingObject_Injected = RemoteSettings.GetAsScriptingObject_Injected(t, defaultValue, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return asScriptingObject_Injected;
		}

		public static IDictionary<string, object> GetDictionary(string key = "")
		{
			RemoteSettings.UseSafeLock();
			IDictionary<string, object> dictionary = RemoteConfigSettingsHelper.GetDictionary(RemoteSettings.GetSafeTopMap(), key);
			RemoteSettings.ReleaseSafeLock();
			return dictionary;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UseSafeLock();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReleaseSafeLock();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetSafeTopMap();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetInt_Injected(ref ManagedSpanWrapper key, [DefaultValue("0")] int defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetLong_Injected(ref ManagedSpanWrapper key, [DefaultValue("0")] long defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloat_Injected(ref ManagedSpanWrapper key, [DefaultValue("0.0F")] float defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetString_Injected(ref ManagedSpanWrapper key, [DefaultValue("\"\"")] ref ManagedSpanWrapper defaultValue, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBool_Injected(ref ManagedSpanWrapper key, [DefaultValue("false")] bool defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasKey_Injected(ref ManagedSpanWrapper key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetAsScriptingObject_Injected(Type t, object defaultValue, ref ManagedSpanWrapper key);

		public delegate void UpdatedEventHandler();
	}
}
