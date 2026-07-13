using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	public static class NativeParallelHashMapExtensions
	{
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public static int Unique<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(this NativeArray<T> array) where T : struct, ValueType, IEquatable<T>
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

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static ValueTuple<NativeArray<TKey>, int> GetUniqueKeyArray<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue>(this UnsafeParallelMultiHashMap<TKey, TValue> container, AllocatorManager.AllocatorHandle allocator) where TKey : struct, ValueType, IEquatable<TKey>, IComparable<TKey> where TValue : struct, ValueType
		{
			NativeArray<TKey> keyArray = container.GetKeyArray(allocator);
			keyArray.Sort<TKey>();
			int num = keyArray.Unique<TKey>();
			return new ValueTuple<NativeArray<TKey>, int>(keyArray, num);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static ValueTuple<NativeArray<TKey>, int> GetUniqueKeyArray<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue>(this NativeParallelMultiHashMap<TKey, TValue> container, AllocatorManager.AllocatorHandle allocator) where TKey : struct, ValueType, IEquatable<TKey>, IComparable<TKey> where TValue : struct, ValueType
		{
			NativeArray<TKey> keyArray = container.GetKeyArray(allocator);
			keyArray.Sort<TKey>();
			int num = keyArray.Unique<TKey>();
			return new ValueTuple<NativeArray<TKey>, int>(keyArray, num);
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static UnsafeParallelHashMapBucketData GetUnsafeBucketData<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue>(this NativeParallelHashMap<TKey, TValue> container) where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType
		{
			return container.m_HashMapData.m_Buffer->GetBucketData();
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static UnsafeParallelHashMapBucketData GetUnsafeBucketData<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue>(this NativeParallelMultiHashMap<TKey, TValue> container) where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType
		{
			return container.m_MultiHashMapData.m_Buffer->GetBucketData();
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static void Remove<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue>(this NativeParallelMultiHashMap<TKey, TValue> container, TKey key, TValue value) where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType, IEquatable<TValue>
		{
			container.m_MultiHashMapData.Remove<TValue>(key, value);
		}
	}
}
