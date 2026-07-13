using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
	public struct AllocatorHelper<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : IDisposable where T : struct, ValueType, AllocatorManager.IAllocator
	{
		public unsafe ref T Allocator
		{
			get
			{
				return UnsafeUtility.AsRef<T>((void*)this.m_allocator);
			}
		}

		[ExcludeFromBurstCompatTesting("CreateAllocator is unburstable")]
		public unsafe AllocatorHelper(AllocatorManager.AllocatorHandle backingAllocator, bool isGlobal = false, int globalIndex = 0)
		{
			ref T ptr = ref AllocatorManager.CreateAllocator<T>(backingAllocator, isGlobal, globalIndex);
			this.m_allocator = (T*)UnsafeUtility.AddressOf<T>(ref ptr);
			this.m_backingAllocator = backingAllocator;
		}

		[ExcludeFromBurstCompatTesting("DestroyAllocator is unburstable")]
		public unsafe void Dispose()
		{
			UnsafeUtility.AsRef<T>((void*)this.m_allocator).DestroyAllocator<T>(this.m_backingAllocator);
		}

		private unsafe readonly T* m_allocator;

		private AllocatorManager.AllocatorHandle m_backingAllocator;
	}
}
