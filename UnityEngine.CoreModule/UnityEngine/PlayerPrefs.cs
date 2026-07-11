using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Stores and accesses player preferences between game sessions.</para>
	/// </summary>
	[NativeHeader("Runtime/Utilities/PlayerPrefs.h")]
	public class PlayerPrefs
	{
		[NativeMethod("SetInt")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TrySetInt(string key, int value);

		[NativeMethod("SetFloat")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TrySetFloat(string key, float value);

		[NativeMethod("SetString")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TrySetSetString(string key, string value);

		/// <summary>
		///   <para>Sets the value of the preference identified by key.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void SetInt(string key, int value)
		{
			if (!PlayerPrefs.TrySetInt(key, value))
			{
				throw new PlayerPrefsException("Could not store preference value");
			}
		}

		/// <summary>
		///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetInt(string key, int defaultValue);

		/// <summary>
		///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		public static int GetInt(string key)
		{
			return PlayerPrefs.GetInt(key, 0);
		}

		/// <summary>
		///   <para>Sets the value of the preference identified by key.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void SetFloat(string key, float value)
		{
			if (!PlayerPrefs.TrySetFloat(key, value))
			{
				throw new PlayerPrefsException("Could not store preference value");
			}
		}

		/// <summary>
		///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetFloat(string key, float defaultValue);

		/// <summary>
		///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		public static float GetFloat(string key)
		{
			return PlayerPrefs.GetFloat(key, 0f);
		}

		/// <summary>
		///   <para>Sets the value of the preference identified by key.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void SetString(string key, string value)
		{
			if (!PlayerPrefs.TrySetSetString(key, value))
			{
				throw new PlayerPrefsException("Could not store preference value");
			}
		}

		/// <summary>
		///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetString(string key, string defaultValue);

		/// <summary>
		///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		public static string GetString(string key)
		{
			return PlayerPrefs.GetString(key, "");
		}

		/// <summary>
		///   <para>Returns true if key exists in the preferences.</para>
		/// </summary>
		/// <param name="key"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasKey(string key);

		/// <summary>
		///   <para>Removes key and its corresponding value from the preferences.</para>
		/// </summary>
		/// <param name="key"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeleteKey(string key);

		/// <summary>
		///   <para>Removes all keys and values from the preferences. Use with caution.</para>
		/// </summary>
		[NativeMethod("DeleteAllWithCallback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeleteAll();

		/// <summary>
		///   <para>Writes all modified preferences to disk.</para>
		/// </summary>
		[NativeMethod("Sync")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Save();
	}
}
