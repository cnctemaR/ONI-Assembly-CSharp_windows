using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[BurstCompatible]
	public static class NativeMultiHashMapExtensions
	{
		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int),
			typeof(AllocatorManager.AllocatorHandle)
		})]
		internal static void Initialize<TKey, TValue, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this NativeMultiHashMap<TKey, TValue> nativeMultiHashMap, int capacity, ref U allocator, int disposeSentinelStackDepth = 2) where TKey : struct, IEquatable<TKey> where TValue : struct where U : struct, ValueType, AllocatorManager.IAllocator
		{
			nativeMultiHashMap.m_MultiHashMapData = new UnsafeMultiHashMap<TKey, TValue>(capacity, allocator.Handle);
		}
	}
}
