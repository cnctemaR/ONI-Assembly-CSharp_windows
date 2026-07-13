using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	public static class ListExtensions
	{
		public static bool RemoveSwapBack<T>(this List<T> list, T value)
		{
			int num = list.IndexOf(value);
			if (num < 0)
			{
				return false;
			}
			list.RemoveAtSwapBack<T>(num);
			return true;
		}

		public static bool RemoveSwapBack<T>(this List<T> list, Predicate<T> matcher)
		{
			int num = list.FindIndex(matcher);
			if (num < 0)
			{
				return false;
			}
			list.RemoveAtSwapBack<T>(num);
			return true;
		}

		public static void RemoveAtSwapBack<T>(this List<T> list, int index)
		{
			int num = list.Count - 1;
			list[index] = list[num];
			list.RemoveAt(num);
		}

		public static NativeList<T> ToNativeList<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this List<T> list, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
		{
			NativeList<T> nativeList = new NativeList<T>(list.Count, allocator);
			for (int i = 0; i < list.Count; i++)
			{
				nativeList.AddNoResize(list[i]);
			}
			return nativeList;
		}

		public static NativeArray<T> ToNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this List<T> list, AllocatorManager.AllocatorHandle allocator) where T : struct, ValueType
		{
			NativeArray<T> nativeArray = CollectionHelper.CreateNativeArray<T>(list.Count, allocator, NativeArrayOptions.UninitializedMemory);
			for (int i = 0; i < list.Count; i++)
			{
				nativeArray[i] = list[i];
			}
			return nativeArray;
		}
	}
}
