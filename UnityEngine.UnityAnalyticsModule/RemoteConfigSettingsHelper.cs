using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal static class RemoteConfigSettingsHelper
	{
		internal unsafe static IntPtr GetSafeMap(IntPtr m, string key)
		{
			IntPtr safeMap_Injected;
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
				safeMap_Injected = RemoteConfigSettingsHelper.GetSafeMap_Injected(m, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return safeMap_Injected;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string[] GetSafeMapKeys(IntPtr m);

		internal static RemoteConfigSettingsHelper.Tag[] GetSafeMapTypes(IntPtr m)
		{
			RemoteConfigSettingsHelper.Tag[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RemoteConfigSettingsHelper.GetSafeMapTypes_Injected(m, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RemoteConfigSettingsHelper.Tag[] array;
				blittableArrayWrapper.Unmarshal<RemoteConfigSettingsHelper.Tag>(ref array);
				array2 = array;
			}
			return array2;
		}

		internal unsafe static long GetSafeNumber(IntPtr m, string key, long defaultValue)
		{
			long safeNumber_Injected;
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
				safeNumber_Injected = RemoteConfigSettingsHelper.GetSafeNumber_Injected(m, ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return safeNumber_Injected;
		}

		internal unsafe static float GetSafeFloat(IntPtr m, string key, float defaultValue)
		{
			float safeFloat_Injected;
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
				safeFloat_Injected = RemoteConfigSettingsHelper.GetSafeFloat_Injected(m, ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return safeFloat_Injected;
		}

		internal unsafe static bool GetSafeBool(IntPtr m, string key, bool defaultValue)
		{
			bool safeBool_Injected;
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
				safeBool_Injected = RemoteConfigSettingsHelper.GetSafeBool_Injected(m, ref managedSpanWrapper, defaultValue);
			}
			finally
			{
				char* ptr = null;
			}
			return safeBool_Injected;
		}

		internal unsafe static string GetSafeStringValue(IntPtr m, string key, string defaultValue)
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
				RemoteConfigSettingsHelper.GetSafeStringValue_Injected(m, ref managedSpanWrapper, ref managedSpanWrapper2, out managedSpanWrapper3);
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

		internal unsafe static IntPtr GetSafeArray(IntPtr m, string key)
		{
			IntPtr safeArray_Injected;
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
				safeArray_Injected = RemoteConfigSettingsHelper.GetSafeArray_Injected(m, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return safeArray_Injected;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long GetSafeArraySize(IntPtr a);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetSafeArrayArray(IntPtr a, long i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetSafeArrayMap(IntPtr a, long i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern RemoteConfigSettingsHelper.Tag GetSafeArrayType(IntPtr a, long i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long GetSafeNumberArray(IntPtr a, long i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetSafeArrayFloat(IntPtr a, long i);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetSafeArrayBool(IntPtr a, long i);

		internal static string GetSafeArrayStringValue(IntPtr a, long i)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				RemoteConfigSettingsHelper.GetSafeArrayStringValue_Injected(a, i, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public static IDictionary<string, object> GetDictionary(IntPtr m, string key)
		{
			bool flag = m == IntPtr.Zero;
			IDictionary<string, object> dictionary;
			if (flag)
			{
				dictionary = null;
			}
			else
			{
				bool flag2 = !string.IsNullOrEmpty(key);
				if (flag2)
				{
					m = RemoteConfigSettingsHelper.GetSafeMap(m, key);
					bool flag3 = m == IntPtr.Zero;
					if (flag3)
					{
						return null;
					}
				}
				dictionary = RemoteConfigSettingsHelper.GetDictionary(m);
			}
			return dictionary;
		}

		internal static IDictionary<string, object> GetDictionary(IntPtr m)
		{
			bool flag = m == IntPtr.Zero;
			IDictionary<string, object> dictionary;
			if (flag)
			{
				dictionary = null;
			}
			else
			{
				IDictionary<string, object> dictionary2 = new Dictionary<string, object>();
				RemoteConfigSettingsHelper.Tag[] safeMapTypes = RemoteConfigSettingsHelper.GetSafeMapTypes(m);
				string[] safeMapKeys = RemoteConfigSettingsHelper.GetSafeMapKeys(m);
				for (int i = 0; i < safeMapKeys.Length; i++)
				{
					RemoteConfigSettingsHelper.SetDictKeyType(m, dictionary2, safeMapKeys[i], safeMapTypes[i]);
				}
				dictionary = dictionary2;
			}
			return dictionary;
		}

		internal static object GetArrayArrayEntries(IntPtr a, long i)
		{
			return RemoteConfigSettingsHelper.GetArrayEntries(RemoteConfigSettingsHelper.GetSafeArrayArray(a, i));
		}

		internal static IDictionary<string, object> GetArrayMapEntries(IntPtr a, long i)
		{
			return RemoteConfigSettingsHelper.GetDictionary(RemoteConfigSettingsHelper.GetSafeArrayMap(a, i));
		}

		internal static T[] GetArrayEntriesType<T>(IntPtr a, long size, Func<IntPtr, long, T> f)
		{
			T[] array = new T[size];
			for (long num = 0L; num < size; num += 1L)
			{
				array[(int)(checked((IntPtr)num))] = f(a, num);
			}
			return array;
		}

		internal static object GetArrayEntries(IntPtr a)
		{
			long safeArraySize = RemoteConfigSettingsHelper.GetSafeArraySize(a);
			bool flag = safeArraySize == 0L;
			object obj;
			if (flag)
			{
				obj = null;
			}
			else
			{
				switch (RemoteConfigSettingsHelper.GetSafeArrayType(a, 0L))
				{
				case RemoteConfigSettingsHelper.Tag.kIntVal:
				case RemoteConfigSettingsHelper.Tag.kInt64Val:
					return RemoteConfigSettingsHelper.GetArrayEntriesType<long>(a, safeArraySize, new Func<IntPtr, long, long>(RemoteConfigSettingsHelper.GetSafeNumberArray));
				case RemoteConfigSettingsHelper.Tag.kDoubleVal:
					return RemoteConfigSettingsHelper.GetArrayEntriesType<float>(a, safeArraySize, new Func<IntPtr, long, float>(RemoteConfigSettingsHelper.GetSafeArrayFloat));
				case RemoteConfigSettingsHelper.Tag.kBoolVal:
					return RemoteConfigSettingsHelper.GetArrayEntriesType<bool>(a, safeArraySize, new Func<IntPtr, long, bool>(RemoteConfigSettingsHelper.GetSafeArrayBool));
				case RemoteConfigSettingsHelper.Tag.kStringVal:
					return RemoteConfigSettingsHelper.GetArrayEntriesType<string>(a, safeArraySize, new Func<IntPtr, long, string>(RemoteConfigSettingsHelper.GetSafeArrayStringValue));
				case RemoteConfigSettingsHelper.Tag.kArrayVal:
					return RemoteConfigSettingsHelper.GetArrayEntriesType<object>(a, safeArraySize, new Func<IntPtr, long, object>(RemoteConfigSettingsHelper.GetArrayArrayEntries));
				case RemoteConfigSettingsHelper.Tag.kMapVal:
					return RemoteConfigSettingsHelper.GetArrayEntriesType<IDictionary<string, object>>(a, safeArraySize, new Func<IntPtr, long, IDictionary<string, object>>(RemoteConfigSettingsHelper.GetArrayMapEntries));
				}
				obj = null;
			}
			return obj;
		}

		internal static object GetMixedArrayEntries(IntPtr a)
		{
			long safeArraySize = RemoteConfigSettingsHelper.GetSafeArraySize(a);
			bool flag = safeArraySize == 0L;
			object obj;
			if (flag)
			{
				obj = null;
			}
			else
			{
				object[] array = new object[safeArraySize];
				for (long num = 0L; num < safeArraySize; num += 1L)
				{
					checked
					{
						switch (RemoteConfigSettingsHelper.GetSafeArrayType(a, num))
						{
						case RemoteConfigSettingsHelper.Tag.kIntVal:
						case RemoteConfigSettingsHelper.Tag.kInt64Val:
							array[(int)((IntPtr)num)] = RemoteConfigSettingsHelper.GetSafeNumberArray(a, num);
							break;
						case RemoteConfigSettingsHelper.Tag.kDoubleVal:
							array[(int)((IntPtr)num)] = RemoteConfigSettingsHelper.GetSafeArrayFloat(a, num);
							break;
						case RemoteConfigSettingsHelper.Tag.kBoolVal:
							array[(int)((IntPtr)num)] = RemoteConfigSettingsHelper.GetSafeArrayBool(a, num);
							break;
						case RemoteConfigSettingsHelper.Tag.kStringVal:
							array[(int)((IntPtr)num)] = RemoteConfigSettingsHelper.GetSafeArrayStringValue(a, num);
							break;
						case RemoteConfigSettingsHelper.Tag.kArrayVal:
							array[(int)((IntPtr)num)] = RemoteConfigSettingsHelper.GetArrayArrayEntries(a, num);
							break;
						case RemoteConfigSettingsHelper.Tag.kMapVal:
							array[(int)((IntPtr)num)] = RemoteConfigSettingsHelper.GetArrayMapEntries(a, num);
							break;
						}
					}
				}
				obj = array;
			}
			return obj;
		}

		internal static void SetDictKeyType(IntPtr m, IDictionary<string, object> dict, string key, RemoteConfigSettingsHelper.Tag tag)
		{
			switch (tag)
			{
			case RemoteConfigSettingsHelper.Tag.kIntVal:
			case RemoteConfigSettingsHelper.Tag.kInt64Val:
				dict[key] = RemoteConfigSettingsHelper.GetSafeNumber(m, key, 0L);
				break;
			case RemoteConfigSettingsHelper.Tag.kDoubleVal:
				dict[key] = RemoteConfigSettingsHelper.GetSafeFloat(m, key, 0f);
				break;
			case RemoteConfigSettingsHelper.Tag.kBoolVal:
				dict[key] = RemoteConfigSettingsHelper.GetSafeBool(m, key, false);
				break;
			case RemoteConfigSettingsHelper.Tag.kStringVal:
				dict[key] = RemoteConfigSettingsHelper.GetSafeStringValue(m, key, "");
				break;
			case RemoteConfigSettingsHelper.Tag.kArrayVal:
				dict[key] = RemoteConfigSettingsHelper.GetArrayEntries(RemoteConfigSettingsHelper.GetSafeArray(m, key));
				break;
			case RemoteConfigSettingsHelper.Tag.kMixedArrayVal:
				dict[key] = RemoteConfigSettingsHelper.GetMixedArrayEntries(RemoteConfigSettingsHelper.GetSafeArray(m, key));
				break;
			case RemoteConfigSettingsHelper.Tag.kMapVal:
				dict[key] = RemoteConfigSettingsHelper.GetDictionary(RemoteConfigSettingsHelper.GetSafeMap(m, key));
				break;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSafeMap_Injected(IntPtr m, ref ManagedSpanWrapper key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSafeMapTypes_Injected(IntPtr m, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetSafeNumber_Injected(IntPtr m, ref ManagedSpanWrapper key, long defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetSafeFloat_Injected(IntPtr m, ref ManagedSpanWrapper key, float defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetSafeBool_Injected(IntPtr m, ref ManagedSpanWrapper key, bool defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSafeStringValue_Injected(IntPtr m, ref ManagedSpanWrapper key, ref ManagedSpanWrapper defaultValue, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSafeArray_Injected(IntPtr m, ref ManagedSpanWrapper key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSafeArrayStringValue_Injected(IntPtr a, long i, out ManagedSpanWrapper ret);

		[RequiredByNativeCode]
		internal enum Tag
		{
			kUnknown,
			kIntVal,
			kInt64Val,
			kUInt64Val,
			kDoubleVal,
			kBoolVal,
			kStringVal,
			kArrayVal,
			kMixedArrayVal,
			kMapVal,
			kMaxTags
		}
	}
}
