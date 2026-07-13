using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Analytics;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/UnityAnalytics/RemoteSettings/RemoteSettings.h")]
	[NativeHeader("UnityAnalyticsScriptingClasses.h")]
	[ExcludeFromDocs]
	[NativeHeader("Modules/UnityAnalyticsCommon/Public/UnityAnalyticsCommon.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class RemoteConfigSettings : IDisposable
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<bool> Updated;

		private RemoteConfigSettings()
		{
		}

		public RemoteConfigSettings(string configKey)
		{
			this.m_Ptr = RemoteConfigSettings.Internal_Create(this, configKey);
			this.Updated = null;
		}

		~RemoteConfigSettings()
		{
			this.Destroy();
		}

		private void Destroy()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
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

		internal unsafe static IntPtr Internal_Create([UnityMarshalAs(NativeType.ScriptingObjectPtr)] RemoteConfigSettings rcs, string configKey)
		{
			IntPtr intPtr;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(configKey, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = configKey.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				intPtr = RemoteConfigSettings.Internal_Create_Injected(rcs, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return intPtr;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_Destroy(IntPtr ptr);

		[RequiredByNativeCode]
		internal static void RemoteConfigSettingsUpdated(RemoteConfigSettings rcs, bool wasLastUpdatedFromServer)
		{
			Action<bool> updated = rcs.Updated;
			bool flag = updated != null;
			if (flag)
			{
				updated(wasLastUpdatedFromServer);
			}
		}

		public unsafe static AnalyticsResult QueueConfig(string name, object param, int ver = 1, string prefix = "")
		{
			AnalyticsResult analyticsResult;
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
				if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = prefix.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				analyticsResult = RemoteConfigSettings.QueueConfig_Injected(ref managedSpanWrapper, param, ver, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return analyticsResult;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SendDeviceInfoInConfigRequest();

		public unsafe static void AddSessionTag(string tag)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tag.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				RemoteConfigSettings.AddSessionTag_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void ForceUpdate()
		{
			IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RemoteConfigSettings.ForceUpdate_Injected(intPtr);
		}

		public bool WasLastUpdatedFromServer()
		{
			IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RemoteConfigSettings.WasLastUpdatedFromServer_Injected(intPtr);
		}

		[ExcludeFromDocs]
		public int GetInt(string key)
		{
			return this.GetInt(key, 0);
		}

		public unsafe int GetInt(string key, [DefaultValue("0")] int defaultValue)
		{
			int int_Injected;
			try
			{
				IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				int_Injected = RemoteConfigSettings.GetInt_Injected(intPtr, ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return int_Injected;
		}

		[ExcludeFromDocs]
		public long GetLong(string key)
		{
			return this.GetLong(key, 0L);
		}

		public unsafe long GetLong(string key, [DefaultValue("0")] long defaultValue)
		{
			long long_Injected;
			try
			{
				IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				long_Injected = RemoteConfigSettings.GetLong_Injected(intPtr, ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return long_Injected;
		}

		[ExcludeFromDocs]
		public float GetFloat(string key)
		{
			return this.GetFloat(key, 0f);
		}

		public unsafe float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue)
		{
			float float_Injected;
			try
			{
				IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				float_Injected = RemoteConfigSettings.GetFloat_Injected(intPtr, ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return float_Injected;
		}

		[ExcludeFromDocs]
		public string GetString(string key)
		{
			return this.GetString(key, "");
		}

		public unsafe string GetString(string key, [DefaultValue("\"\"")] string defaultValue)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
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
				RemoteConfigSettings.GetString_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2, out managedSpanWrapper3);
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
		public bool GetBool(string key)
		{
			return this.GetBool(key, false);
		}

		public unsafe bool GetBool(string key, [DefaultValue("false")] bool defaultValue)
		{
			bool bool_Injected;
			try
			{
				IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				bool_Injected = RemoteConfigSettings.GetBool_Injected(intPtr, ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return bool_Injected;
		}

		public unsafe bool HasKey(string key)
		{
			bool flag;
			try
			{
				IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = RemoteConfigSettings.HasKey_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public int GetCount()
		{
			IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RemoteConfigSettings.GetCount_Injected(intPtr);
		}

		public string[] GetKeys()
		{
			IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RemoteConfigSettings.GetKeys_Injected(intPtr);
		}

		public T GetObject<T>(string key = "")
		{
			return (T)((object)this.GetObject(typeof(T), key));
		}

		public object GetObject(Type type, string key = "")
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
			return this.GetAsScriptingObject(type, null, key);
		}

		public object GetObject(string key, object defaultValue)
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
			return this.GetAsScriptingObject(type, defaultValue, key);
		}

		internal unsafe object GetAsScriptingObject(Type t, object defaultValue, string key)
		{
			object asScriptingObject_Injected;
			try
			{
				IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				asScriptingObject_Injected = RemoteConfigSettings.GetAsScriptingObject_Injected(intPtr, t, defaultValue, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return asScriptingObject_Injected;
		}

		public IDictionary<string, object> GetDictionary(string key = "")
		{
			this.UseSafeLock();
			IDictionary<string, object> dictionary = RemoteConfigSettingsHelper.GetDictionary(this.GetSafeTopMap(), key);
			this.ReleaseSafeLock();
			return dictionary;
		}

		internal void UseSafeLock()
		{
			IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RemoteConfigSettings.UseSafeLock_Injected(intPtr);
		}

		internal void ReleaseSafeLock()
		{
			IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RemoteConfigSettings.ReleaseSafeLock_Injected(intPtr);
		}

		internal IntPtr GetSafeTopMap()
		{
			IntPtr intPtr = RemoteConfigSettings.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RemoteConfigSettings.GetSafeTopMap_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create_Injected(RemoteConfigSettings rcs, ref ManagedSpanWrapper configKey);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnalyticsResult QueueConfig_Injected(ref ManagedSpanWrapper name, object param, int ver, ref ManagedSpanWrapper prefix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddSessionTag_Injected(ref ManagedSpanWrapper tag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ForceUpdate_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool WasLastUpdatedFromServer_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetInt_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, [DefaultValue("0")] int defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetLong_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, [DefaultValue("0")] long defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloat_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, [DefaultValue("0.0F")] float defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, [DefaultValue("\"\"")] ref ManagedSpanWrapper defaultValue, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBool_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, [DefaultValue("false")] bool defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasKey_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetKeys_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetAsScriptingObject_Injected(IntPtr _unity_self, Type t, object defaultValue, ref ManagedSpanWrapper key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UseSafeLock_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseSafeLock_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSafeTopMap_Injected(IntPtr _unity_self);

		[NonSerialized]
		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(RemoteConfigSettings remoteConfigSettings)
			{
				return remoteConfigSettings.m_Ptr;
			}
		}
	}
}
