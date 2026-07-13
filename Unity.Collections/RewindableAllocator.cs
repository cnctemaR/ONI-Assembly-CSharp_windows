using System;
using System.Runtime.CompilerServices;
using System.Threading;
using AOT;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections
{
	[BurstCompile]
	public struct RewindableAllocator : AllocatorManager.IAllocator, IDisposable
	{
		public unsafe void Initialize(int initialSizeInBytes, bool enableBlockFree = false)
		{
			this.m_spinner = default(Spinner);
			this.m_block = new UnmanagedArray<RewindableAllocator.MemoryBlock>(64, Allocator.Persistent);
			*this.m_block[0] = new RewindableAllocator.MemoryBlock((long)initialSizeInBytes);
			this.m_last = (this.m_used = (this.m_best = 0));
			this.m_enableBlockFree = enableBlockFree;
		}

		public bool EnableBlockFree
		{
			get
			{
				return this.m_enableBlockFree;
			}
			set
			{
				this.m_enableBlockFree = value;
			}
		}

		public int BlocksAllocated
		{
			get
			{
				return this.m_last + 1;
			}
		}

		public int InitialSizeInBytes
		{
			get
			{
				return (int)this.m_block[0].m_bytes;
			}
		}

		public void Rewind()
		{
			if (JobsUtility.IsExecutingJob)
			{
				throw new InvalidOperationException("You cannot Rewind a RewindableAllocator from a Job.");
			}
			this.m_handle.Rewind();
			while (this.m_last > this.m_used)
			{
				int num = this.m_last;
				this.m_last = num - 1;
				this.m_block[num].Dispose();
			}
			while (this.m_used > 0)
			{
				int num = this.m_used;
				this.m_used = num - 1;
				this.m_block[num].Rewind();
			}
			this.m_block[0].Rewind();
		}

		public void Dispose()
		{
			if (JobsUtility.IsExecutingJob)
			{
				throw new InvalidOperationException("You cannot Dispose a RewindableAllocator from a Job.");
			}
			this.m_used = 0;
			this.Rewind();
			this.m_block[0].Dispose();
			this.m_block.Dispose();
			this.m_last = (this.m_used = (this.m_best = 0));
		}

		[NotBurstCompatible]
		public AllocatorManager.TryFunction Function
		{
			get
			{
				return new AllocatorManager.TryFunction(RewindableAllocator.Try);
			}
		}

		public unsafe int Try(ref AllocatorManager.Block block)
		{
			if (block.Range.Pointer == IntPtr.Zero)
			{
				int num = this.m_block[this.m_best].TryAllocate(ref block);
				if (num == 0)
				{
					return num;
				}
				this.m_spinner.Lock();
				int i;
				for (i = 0; i <= this.m_last; i++)
				{
					num = this.m_block[i].TryAllocate(ref block);
					if (num == 0)
					{
						this.m_used = ((i > this.m_used) ? i : this.m_used);
						this.m_best = i;
						this.m_spinner.Unlock();
						return num;
					}
				}
				long num2 = math.max(this.m_block[0].m_bytes << i, math.ceilpow2(block.Bytes));
				*this.m_block[i] = new RewindableAllocator.MemoryBlock(num2);
				num = this.m_block[i].TryAllocate(ref block);
				this.m_best = i;
				this.m_used = i;
				this.m_last = i;
				this.m_spinner.Unlock();
				return num;
			}
			else
			{
				if (block.Range.Items == 0)
				{
					if (this.m_enableBlockFree)
					{
						this.m_spinner.Lock();
						if (this.m_block[this.m_best].Contains(block.Range.Pointer) && Interlocked.Decrement(ref this.m_block[this.m_best].m_allocations) == 0L)
						{
							this.m_block[this.m_best].Rewind();
						}
						this.m_spinner.Unlock();
					}
					return 0;
				}
				return -1;
			}
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
		internal static int Try(IntPtr state, ref AllocatorManager.Block block)
		{
			return RewindableAllocator.Try_000006E8$BurstDirectCall.Invoke(state, ref block);
		}

		public AllocatorManager.AllocatorHandle Handle
		{
			get
			{
				return this.m_handle;
			}
			set
			{
				this.m_handle = value;
			}
		}

		public Allocator ToAllocator
		{
			get
			{
				return this.m_handle.ToAllocator;
			}
		}

		public bool IsCustomAllocator
		{
			get
			{
				return this.m_handle.IsCustomAllocator;
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public NativeArray<T> AllocateNativeArray<T>(int length) where T : struct
		{
			return new NativeArray<T>
			{
				m_Buffer = (ref this).AllocateStruct(default(T), length),
				m_Length = length,
				m_AllocatorLabel = Allocator.None
			};
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe NativeList<T> AllocateNativeList<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(int capacity) where T : struct, ValueType
		{
			NativeList<T> nativeList = default(NativeList<T>);
			nativeList.m_ListData = (ref this).Allocate(default(UnsafeList<T>), 1);
			nativeList.m_ListData->Ptr = (ref this).Allocate(default(T), capacity);
			nativeList.m_ListData->m_capacity = capacity;
			nativeList.m_ListData->m_length = 0;
			nativeList.m_ListData->Allocator = Allocator.None;
			nativeList.m_DeprecatedAllocator = Allocator.None;
			return nativeList;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Try$BurstManaged(IntPtr state, ref AllocatorManager.Block block)
		{
			return ((RewindableAllocator*)(void*)state)->Try(ref block);
		}

		private Spinner m_spinner;

		private AllocatorManager.AllocatorHandle m_handle;

		private UnmanagedArray<RewindableAllocator.MemoryBlock> m_block;

		private int m_best;

		private int m_last;

		private int m_used;

		private bool m_enableBlockFree;

		[BurstCompatible]
		internal struct MemoryBlock : IDisposable
		{
			public unsafe MemoryBlock(long bytes)
			{
				this.m_pointer = (byte*)Memory.Unmanaged.Allocate(bytes, 16384, Allocator.Persistent);
				this.m_bytes = bytes;
				this.m_current = 0L;
				this.m_allocations = 0L;
			}

			public void Rewind()
			{
				this.m_current = 0L;
				this.m_allocations = 0L;
			}

			public void Dispose()
			{
				Memory.Unmanaged.Free<byte>(this.m_pointer, Allocator.Persistent);
				this.m_pointer = null;
				this.m_bytes = 0L;
				this.m_current = 0L;
				this.m_allocations = 0L;
			}

			public unsafe int TryAllocate(ref AllocatorManager.Block block)
			{
				int num = math.max(64, block.Alignment);
				int num2 = ((num != 64) ? 1 : 0);
				int num3 = 63;
				if (num2 == 1)
				{
					num = (num + num3) & ~num3;
				}
				long num4 = (long)num - 1L;
				long num5 = (block.Bytes + (long)(num2 * num) + num4) & ~num4;
				long num6 = Interlocked.Add(ref this.m_current, num5) - num5;
				num6 = (num6 + num4) & ~num4;
				if (num6 + block.Bytes > this.m_bytes)
				{
					return -1;
				}
				block.Range.Pointer = (IntPtr)((void*)(this.m_pointer + num6));
				block.AllocatedItems = block.Range.Items;
				Interlocked.Increment(ref this.m_allocations);
				return 0;
			}

			public unsafe bool Contains(IntPtr ptr)
			{
				void* ptr2 = (void*)ptr;
				return ptr2 >= (void*)this.m_pointer && ptr2 < (void*)(this.m_pointer + this.m_current);
			}

			public const int kMaximumAlignment = 16384;

			public unsafe byte* m_pointer;

			public long m_bytes;

			public long m_current;

			public long m_allocations;
		}

		public delegate int Try_000006E8$PostfixBurstDelegate(IntPtr state, ref AllocatorManager.Block block);

		internal static class Try_000006E8$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (RewindableAllocator.Try_000006E8$BurstDirectCall.Pointer == 0)
				{
					RewindableAllocator.Try_000006E8$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(RewindableAllocator.Try_000006E8$BurstDirectCall.DeferredCompilation, methodof(RewindableAllocator.Try$BurstManaged(IntPtr, ref AllocatorManager.Block)).MethodHandle, typeof(RewindableAllocator.Try_000006E8$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = RewindableAllocator.Try_000006E8$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				RewindableAllocator.Try_000006E8$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			public static void Constructor()
			{
				RewindableAllocator.Try_000006E8$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(RewindableAllocator.Try(IntPtr, ref AllocatorManager.Block)).MethodHandle);
			}

			public static void Initialize()
			{
			}

			// Note: this type is marked as 'beforefieldinit'.
			static Try_000006E8$BurstDirectCall()
			{
				RewindableAllocator.Try_000006E8$BurstDirectCall.Constructor();
			}

			public static int Invoke(IntPtr state, ref AllocatorManager.Block block)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = RewindableAllocator.Try_000006E8$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(System.IntPtr,Unity.Collections.AllocatorManager/Block&), state, ref block, functionPointer);
					}
				}
				return RewindableAllocator.Try$BurstManaged(state, ref block);
			}

			private static IntPtr Pointer;

			private static IntPtr DeferredCompilation;
		}
	}
}
