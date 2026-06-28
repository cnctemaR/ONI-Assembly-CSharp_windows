using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public static class RemoteSettings
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event RemoteSettings.UpdatedEventHandler Updated;

		[RequiredByNativeCode]
		public static void CallOnUpdate()
		{
			RemoteSettings.UpdatedEventHandler updated = RemoteSettings.Updated;
			if (updated != null)
			{
				updated();
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetInt(string key, [DefaultValue("0")] int defaultValue);

		[ExcludeFromDocs]
		public static int GetInt(string key)
		{
			int num = 0;
			return RemoteSettings.GetInt(key, num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue);

		[ExcludeFromDocs]
		public static float GetFloat(string key)
		{
			float num = 0f;
			return RemoteSettings.GetFloat(key, num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetString(string key, [DefaultValue("\"\"")] string defaultValue);

		[ExcludeFromDocs]
		public static string GetString(string key)
		{
			string text = "";
			return RemoteSettings.GetString(key, text);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetBool(string key, [DefaultValue("false")] bool defaultValue);

		[ExcludeFromDocs]
		public static bool GetBool(string key)
		{
			bool flag = false;
			return RemoteSettings.GetBool(key, flag);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasKey(string key);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetKeys();

		public delegate void UpdatedEventHandler();
	}
}
