using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
			long num = (((long)initialSizeInBytes > 131072L) ? ((long)initialSizeInBytes) : 131072L);
			*this.m_block[0] = new RewindableAllocator.MemoryBlock(num);
			this.m_last = (this.m_used = 0);
			this.m_enableBlockFree = (enableBlockFree ? 1 : 0);
			this.m_reachMaxBlockSize = (((long)initialSizeInBytes >= 67108864L) ? 1 : 0);
		}

		public bool EnableBlockFree
		{
			get
			{
				return this.m_enableBlockFree > 0;
			}
			set
			{
				this.m_enableBlockFree = (value ? 1 : 0);
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

		internal long MaxMemoryBlockSize
		{
			get
			{
				return 67108864L;
			}
		}

		internal long BytesAllocated
		{
			get
			{
				long num = 0L;
				for (int i = 0; i <= this.m_last; i++)
				{
					num += this.m_block[i].m_bytes;
				}
				return num;
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
			this.m_last = (this.m_used = 0);
		}

		[ExcludeFromBurstCompatTesting("Uses managed delegate")]
		public AllocatorManager.TryFunction Function
		{
			get
			{
				return new AllocatorManager.TryFunction(RewindableAllocator.Try);
			}
		}

		private unsafe int TryAllocate(ref AllocatorManager.Block block, int startIndex, int lastIndex, long alignedSize, long alignmentMask)
		{
			int i = startIndex;
			while (i <= lastIndex)
			{
				RewindableAllocator.Union union = default(RewindableAllocator.Union);
				bool flag = false;
				union.m_long = Interlocked.Read(ref this.m_block[i].m_union.m_long);
				long num;
				RewindableAllocator.Union union2;
				do
				{
					num = (union.m_current + alignmentMask) & ~alignmentMask;
					if (num + block.Bytes > this.m_block[i].m_bytes)
					{
						goto Block_1;
					}
					union2 = union;
					RewindableAllocator.Union union3 = default(RewindableAllocator.Union);
					union3.m_current = ((num + alignedSize > this.m_block[i].m_bytes) ? this.m_block[i].m_bytes : (num + alignedSize));
					union3.m_allocCount = union.m_allocCount + 1L;
					union.m_long = Interlocked.CompareExchange(ref this.m_block[i].m_union.m_long, union3.m_long, union2.m_long);
				}
				while (union.m_long != union2.m_long);
				IL_00F9:
				if (!flag)
				{
					block.Range.Pointer = (IntPtr)((void*)(this.m_block[i].m_pointer + num));
					block.AllocatedItems = block.Range.Items;
					Interlocked.MemoryBarrier();
					int num2 = this.m_used;
					int num3;
					int num4;
					do
					{
						num3 = num2;
						num4 = ((i > num3) ? i : num3);
						num2 = Interlocked.CompareExchange(ref this.m_used, num4, num3);
					}
					while (num4 != num3);
					return 0;
				}
				i++;
				continue;
				Block_1:
				flag = true;
				goto IL_00F9;
			}
			return -1;
		}

		public unsafe int Try(ref AllocatorManager.Block block)
		{
			if (block.Range.Pointer == IntPtr.Zero)
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
				int last = this.m_last;
				int num6 = this.TryAllocate(ref block, 0, this.m_last, num5, num4);
				if (num6 == 0)
				{
					return num6;
				}
				this.m_spinner.Acquire();
				num6 = this.TryAllocate(ref block, last, this.m_last, num5, num4);
				if (num6 == 0)
				{
					this.m_spinner.Release();
					return num6;
				}
				long num7;
				if (this.m_reachMaxBlockSize == 0)
				{
					num7 = this.m_block[this.m_last].m_bytes << 1;
				}
				else
				{
					num7 = this.m_block[this.m_last].m_bytes + 67108864L;
				}
				num7 = math.max(num7, num5);
				this.m_reachMaxBlockSize = ((num7 >= 67108864L) ? 1 : 0);
				*this.m_block[this.m_last + 1] = new RewindableAllocator.MemoryBlock(num7);
				Interlocked.Increment(ref this.m_last);
				num6 = this.TryAllocate(ref block, this.m_last, this.m_last, num5, num4);
				this.m_spinner.Release();
				return num6;
			}
			else
			{
				if (block.Range.Items == 0)
				{
					if (this.m_enableBlockFree != 0)
					{
						for (int i = 0; i <= this.m_last; i++)
						{
							if (this.m_block[i].Contains(block.Range.Pointer))
							{
								RewindableAllocator.Union union = default(RewindableAllocator.Union);
								union.m_long = Interlocked.Read(ref this.m_block[i].m_union.m_long);
								RewindableAllocator.Union union2;
								do
								{
									union2 = union;
									RewindableAllocator.Union union3 = union;
									long allocCount = union3.m_allocCount;
									union3.m_allocCount = allocCount - 1L;
									if (union3.m_allocCount == 0L)
									{
										union3.m_current = 0L;
									}
									union.m_long = Interlocked.CompareExchange(ref this.m_block[i].m_union.m_long, union3.m_long, union2.m_long);
								}
								while (union.m_long != union2.m_long);
							}
						}
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
			return RewindableAllocator.Try_000009E0$BurstDirectCall.Invoke(state, ref block);
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

		public bool IsAutoDispose
		{
			get
			{
				return true;
			}
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public NativeArray<T> AllocateNativeArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(int length) where T : struct, ValueType
		{
			return new NativeArray<T>
			{
				m_Buffer = (ref this).AllocateStruct(default(T), length),
				m_Length = length,
				m_AllocatorLabel = Allocator.None
			};
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe NativeList<T> AllocateNativeList<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(int capacity) where T : struct, ValueType
		{
			NativeList<T> nativeList = default(NativeList<T>);
			nativeList.m_ListData = (ref this).Allocate(default(UnsafeList<T>), 1);
			nativeList.m_ListData->Ptr = (ref this).Allocate(default(T), capacity);
			nativeList.m_ListData->m_length = 0;
			nativeList.m_ListData->m_capacity = capacity;
			nativeList.m_ListData->Allocator = Allocator.None;
			return nativeList;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Try$BurstManaged(IntPtr state, ref AllocatorManager.Block block)
		{
			return ((RewindableAllocator*)(void*)state)->Try(ref block);
		}

		private const int kLog2MaxMemoryBlockSize = 26;

		private const long kMaxMemoryBlockSize = 67108864L;

		private const long kMinMemoryBlockSize = 131072L;

		private const int kMaxNumBlocks = 64;

		private const int kBlockBusyRewindMask = -2147483648;

		private const int kBlockBusyAllocateMask = 2147483647;

		private Spinner m_spinner;

		private AllocatorManager.AllocatorHandle m_handle;

		private UnmanagedArray<RewindableAllocator.MemoryBlock> m_block;

		private int m_last;

		private int m_used;

		private byte m_enableBlockFree;

		private byte m_reachMaxBlockSize;

		internal struct Union
		{
			internal long m_current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_long & 1099511627775L;
				}
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				set
				{
					this.m_long &= -1099511627776L;
					this.m_long |= value & 1099511627775L;
				}
			}

			internal long m_allocCount
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return (this.m_long >> 40) & 16777215L;
				}
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				set
				{
					this.m_long &= 1099511627775L;
					this.m_long |= (value & 16777215L) << 40;
				}
			}

			internal long m_long;

			private const int currentBits = 40;

			private const int currentOffset = 0;

			private const long currentMask = 1099511627775L;

			private const int allocCountBits = 24;

			private const int allocCountOffset = 40;

			private const long allocCountMask = 16777215L;
		}

		[GenerateTestsForBurstCompatibility]
		internal struct MemoryBlock : IDisposable
		{
			public unsafe MemoryBlock(long bytes)
			{
				this.m_pointer = (byte*)Memory.Unmanaged.Allocate(bytes, 16384, Allocator.Persistent);
				this.m_bytes = bytes;
				this.m_union = default(RewindableAllocator.Union);
			}

			public void Rewind()
			{
				this.m_union = default(RewindableAllocator.Union);
			}

			public void Dispose()
			{
				Memory.Unmanaged.Free<byte>(this.m_pointer, Allocator.Persistent);
				this.m_pointer = null;
				this.m_bytes = 0L;
				this.m_union = default(RewindableAllocator.Union);
			}

			public unsafe bool Contains(IntPtr ptr)
			{
				void* ptr2 = (void*)ptr;
				return ptr2 >= (void*)this.m_pointer && ptr2 < (void*)(this.m_pointer + this.m_union.m_current);
			}

			public const int kMaximumAlignment = 16384;

			public unsafe byte* m_pointer;

			public long m_bytes;

			public RewindableAllocator.Union m_union;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate int Try_000009E0$PostfixBurstDelegate(IntPtr state, ref AllocatorManager.Block block);

		internal static class Try_000009E0$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (RewindableAllocator.Try_000009E0$BurstDirectCall.Pointer == 0)
				{
					RewindableAllocator.Try_000009E0$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<RewindableAllocator.Try_000009E0$PostfixBurstDelegate>(new RewindableAllocator.Try_000009E0$PostfixBurstDelegate(RewindableAllocator.Try)).Value;
				}
				A_0 = RewindableAllocator.Try_000009E0$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				RewindableAllocator.Try_000009E0$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			public static int Invoke(IntPtr state, ref AllocatorManager.Block block)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = RewindableAllocator.Try_000009E0$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(System.IntPtr,Unity.Collections.AllocatorManager/Block&), state, ref block, functionPointer);
					}
				}
				return RewindableAllocator.Try$BurstManaged(state, ref block);
			}

			private static IntPtr Pointer;
		}
	}
}
