using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	public static class NativeParallelMultiHashMapExtensions
	{
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int),
			typeof(AllocatorManager.AllocatorHandle)
		})]
		internal static void Initialize<[global::System.Runtime.CompilerServices.IsUnmanaged] TKey, [global::System.Runtime.CompilerServices.IsUnmanaged] TValue, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(this NativeParallelMultiHashMap<TKey, TValue> container, int capacity, ref U allocator) where TKey : struct, ValueType, IEquatable<TKey> where TValue : struct, ValueType where U : struct, ValueType, AllocatorManager.IAllocator
		{
			container.m_MultiHashMapData = new UnsafeParallelMultiHashMap<TKey, TValue>(capacity, allocator.Handle);
		}
	}
}
