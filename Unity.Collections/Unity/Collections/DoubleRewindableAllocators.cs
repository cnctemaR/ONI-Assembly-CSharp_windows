using System;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	public struct DoubleRewindableAllocators : IDisposable
	{
		public unsafe void Update()
		{
			RewindableAllocator* ptr = (RewindableAllocator*)UnsafeUtility.AddressOf<RewindableAllocator>(this.UpdateAllocatorHelper0.Allocator);
			RewindableAllocator* ptr2 = (RewindableAllocator*)UnsafeUtility.AddressOf<RewindableAllocator>(this.UpdateAllocatorHelper1.Allocator);
			this.Pointer = ((this.Pointer == ptr) ? ptr2 : ptr);
			this.Allocator.Rewind();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private void CheckIsCreated()
		{
			if (!this.IsCreated)
			{
				throw new InvalidOperationException("DoubleRewindableAllocators is not created.");
			}
		}

		public unsafe ref RewindableAllocator Allocator
		{
			get
			{
				return UnsafeUtility.AsRef<RewindableAllocator>((void*)this.Pointer);
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.Pointer != null;
			}
		}

		public DoubleRewindableAllocators(AllocatorManager.AllocatorHandle backingAllocator, int initialSizeInBytes)
		{
			this = default(DoubleRewindableAllocators);
			this.Initialize(backingAllocator, initialSizeInBytes);
		}

		public void Initialize(AllocatorManager.AllocatorHandle backingAllocator, int initialSizeInBytes)
		{
			this.UpdateAllocatorHelper0 = new AllocatorHelper<RewindableAllocator>(backingAllocator, false, 0);
			this.UpdateAllocatorHelper1 = new AllocatorHelper<RewindableAllocator>(backingAllocator, false, 0);
			this.UpdateAllocatorHelper0.Allocator.Initialize(initialSizeInBytes, false);
			this.UpdateAllocatorHelper1.Allocator.Initialize(initialSizeInBytes, false);
			this.Pointer = null;
			this.Update();
		}

		public void Dispose()
		{
			if (!this.IsCreated)
			{
				return;
			}
			this.UpdateAllocatorHelper0.Allocator.Dispose();
			this.UpdateAllocatorHelper1.Allocator.Dispose();
			this.UpdateAllocatorHelper0.Dispose();
			this.UpdateAllocatorHelper1.Dispose();
		}

		internal bool EnableBlockFree
		{
			get
			{
				return this.UpdateAllocatorHelper0.Allocator.EnableBlockFree;
			}
			set
			{
				this.UpdateAllocatorHelper0.Allocator.EnableBlockFree = value;
				this.UpdateAllocatorHelper1.Allocator.EnableBlockFree = value;
			}
		}

		private unsafe RewindableAllocator* Pointer;

		private AllocatorHelper<RewindableAllocator> UpdateAllocatorHelper0;

		private AllocatorHelper<RewindableAllocator> UpdateAllocatorHelper1;
	}
}
