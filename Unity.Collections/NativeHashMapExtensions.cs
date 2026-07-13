using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[BurstCompatible]
	public static class NativeHashMapExtensions
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static int Unique<T>(this NativeArray<T> array) where T : struct, IEquatable<T>
		{
			if (array.Length == 0)
			{
				return 0;
			}
			int num = 0;
			int length = array.Length;
			int num2 = num;
			while (++num != length)
			{
				T t = array[num2];
				if (!t.Equals(array[num]))
				{
					array[++num2] = array[num];
				}
			}
			return num2 + 1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static ValueTuple<NativeArray<TKey>, int> GetUniqueKeyArray<TKey, TValue>(this UnsafeMultiHashMap<TKey, TValue> container, AllocatorManager.AllocatorHandle allocator) where TKey : struct, IEquatable<TKey>, IComparable<TKey> where TValue : struct
		{
			NativeArray<TKey> keyArray = container.GetKeyArray(allocator);
			keyArray.Sort<TKey>();
			int num = keyArray.Unique<TKey>();
			return new ValueTuple<NativeArray<TKey>, int>(keyArray, num);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static ValueTuple<NativeArray<TKey>, int> GetUniqueKeyArray<TKey, TValue>(this NativeMultiHashMap<TKey, TValue> container, AllocatorManager.AllocatorHandle allocator) where TKey : struct, IEquatable<TKey>, IComparable<TKey> where TValue : struct
		{
			NativeArray<TKey> keyArray = container.GetKeyArray(allocator);
			keyArray.Sort<TKey>();
			int num = keyArray.Unique<TKey>();
			return new ValueTuple<NativeArray<TKey>, int>(keyArray, num);
		}

		[Obsolete("GetBucketData is deprecated, please use GetUnsafeBucketData instead. (RemovedAfter 2021-07-08) (UnityUpgradable) -> GetUnsafeBucketData<TKey,TValue>(*)", false)]
		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static UnsafeHashMapBucketData GetBucketData<TKey, TValue>(this NativeHashMap<TKey, TValue> container) where TKey : struct, IEquatable<TKey> where TValue : struct
		{
			return container.m_HashMapData.m_Buffer->GetBucketData();
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static UnsafeHashMapBucketData GetUnsafeBucketData<TKey, TValue>(this NativeHashMap<TKey, TValue> container) where TKey : struct, IEquatable<TKey> where TValue : struct
		{
			return container.m_HashMapData.m_Buffer->GetBucketData();
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static UnsafeHashMapBucketData GetUnsafeBucketData<TKey, TValue>(this NativeMultiHashMap<TKey, TValue> container) where TKey : struct, IEquatable<TKey> where TValue : struct
		{
			return container.m_MultiHashMapData.m_Buffer->GetBucketData();
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static void Remove<TKey, TValue>(this NativeMultiHashMap<TKey, TValue> container, TKey key, TValue value) where TKey : struct, IEquatable<TKey> where TValue : struct, IEquatable<TValue>
		{
			container.m_MultiHashMapData.Remove<TValue>(key, value);
		}
	}
}
