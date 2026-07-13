using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections.NotBurstCompatible
{
	public static class Extensions
	{
		[NotBurstCompatible]
		public static T[] ToArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeHashSet<T> set) where T : struct, ValueType, IEquatable<T>
		{
			NativeArray<T> nativeArray = set.ToNativeArray(Allocator.TempJob);
			T[] array = nativeArray.ToArray();
			nativeArray.Dispose();
			return array;
		}

		[NotBurstCompatible]
		public static T[] ToArrayNBC<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
		{
			return list.AsArray().ToArray();
		}

		[NotBurstCompatible]
		public static void CopyFromNBC<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeList<T> list, T[] array) where T : struct, ValueType
		{
			list.Clear();
			list.Resize(array.Length, NativeArrayOptions.UninitializedMemory);
			list.AsArray().CopyFrom(array);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		[Obsolete("Burst now supports tuple, please use `GetUniqueKeyArray` method from `Unity.Collections.UnsafeMultiHashMap` instead.", false)]
		public static ValueTuple<NativeArray<TKey>, int> GetUniqueKeyArrayNBC<TKey, TValue>(this UnsafeMultiHashMap<TKey, TValue> hashmap, AllocatorManager.AllocatorHandle allocator) where TKey : struct, IEquatable<TKey>, IComparable<TKey> where TValue : struct
		{
			return hashmap.GetUniqueKeyArray<TKey, TValue>(allocator);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		[Obsolete("Burst now supports tuple, please use `GetUniqueKeyArray` method from `Unity.Collections.NativeMultiHashMap` instead.", false)]
		public static ValueTuple<NativeArray<TKey>, int> GetUniqueKeyArrayNBC<TKey, TValue>(this NativeMultiHashMap<TKey, TValue> hashmap, AllocatorManager.AllocatorHandle allocator) where TKey : struct, IEquatable<TKey>, IComparable<TKey> where TValue : struct
		{
			return hashmap.GetUniqueKeyArray<TKey, TValue>(allocator);
		}
	}
}
