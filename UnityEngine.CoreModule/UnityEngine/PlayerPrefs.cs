using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Utilities/PlayerPrefs.h")]
	public class PlayerPrefs
	{
		[NativeMethod("SetInt")]
		private unsafe static bool TrySetInt(string key, int value)
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
				flag = PlayerPrefs.TrySetInt_Injected(ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeMethod("SetFloat")]
		private unsafe static bool TrySetFloat(string key, float value)
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
				flag = PlayerPrefs.TrySetFloat_Injected(ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeMethod("SetString")]
		private unsafe static bool TrySetSetString(string key, string value)
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
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = value.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				flag = PlayerPrefs.TrySetSetString_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return flag;
		}

		public static void SetInt(string key, int value)
		{
			bool flag = !PlayerPrefs.TrySetInt(key, value);
			if (flag)
			{
				throw new PlayerPrefsException("Could not store preference value");
			}
		}

		public unsafe static int GetInt(string key, int defaultValue)
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
				int_Injected = PlayerPrefs.GetInt_Injected(ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return int_Injected;
		}

		public static int GetInt(string key)
		{
			return PlayerPrefs.GetInt(key, 0);
		}

		public static void SetFloat(string key, float value)
		{
			bool flag = !PlayerPrefs.TrySetFloat(key, value);
			if (flag)
			{
				throw new PlayerPrefsException("Could not store preference value");
			}
		}

		public unsafe static float GetFloat(string key, float defaultValue)
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
				float_Injected = PlayerPrefs.GetFloat_Injected(ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return float_Injected;
		}

		public static float GetFloat(string key)
		{
			return PlayerPrefs.GetFloat(key, 0f);
		}

		public static void SetString(string key, string value)
		{
			bool flag = !PlayerPrefs.TrySetSetString(key, value);
			if (flag)
			{
				throw new PlayerPrefsException("Could not store preference value");
			}
		}

		public unsafe static string GetString(string key, string defaultValue)
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
				PlayerPrefs.GetString_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, out managedSpanWrapper3);
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

		public static string GetString(string key)
		{
			return PlayerPrefs.GetString(key, "");
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
				flag = PlayerPrefs.HasKey_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public unsafe static void DeleteKey(string key)
		{
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
				PlayerPrefs.DeleteKey_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeMethod("DeleteAllWithCallback")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeleteAll();

		[NativeMethod("Sync")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Save();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TrySetInt_Injected(ref ManagedSpanWrapper key, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TrySetFloat_Injected(ref ManagedSpanWrapper key, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TrySetSetString_Injected(ref ManagedSpanWrapper key, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetInt_Injected(ref ManagedSpanWrapper key, int defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloat_Injected(ref ManagedSpanWrapper key, float defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetString_Injected(ref ManagedSpanWrapper key, ref ManagedSpanWrapper defaultValue, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasKey_Injected(ref ManagedSpanWrapper key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DeleteKey_Injected(ref ManagedSpanWrapper key);
	}
}
