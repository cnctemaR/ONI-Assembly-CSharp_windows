using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;

namespace Unity.Collections
{
	[BurstCompile]
	internal struct AutoFreeAllocator : AllocatorManager.IAllocator, IDisposable
	{
		public unsafe void Update()
		{
			int length = this.m_tofree.Length;
			while (length-- > 0)
			{
				int length2 = this.m_allocated.Length;
				while (length2-- > 0)
				{
					if (*this.m_allocated[length2] == *this.m_tofree[length])
					{
						Memory.Unmanaged.Free((void*)(*this.m_tofree[length]), this.m_backingAllocatorHandle);
						this.m_allocated.RemoveAtSwapBack(length2);
						break;
					}
				}
			}
			this.m_tofree.Rewind();
			this.m_allocated.TrimExcess();
		}

		public void Initialize(AllocatorManager.AllocatorHandle backingAllocatorHandle)
		{
			this.m_allocated = new ArrayOfArrays<IntPtr>(1048576, backingAllocatorHandle, 12);
			this.m_tofree = new ArrayOfArrays<IntPtr>(131072, backingAllocatorHandle, 12);
			this.m_backingAllocatorHandle = backingAllocatorHandle;
		}

		public unsafe void FreeAll()
		{
			this.Update();
			this.m_handle.Rewind();
			for (int i = 0; i < this.m_allocated.Length; i++)
			{
				Memory.Unmanaged.Free((void*)(*this.m_allocated[i]), this.m_backingAllocatorHandle);
			}
			this.m_allocated.Rewind();
		}

		public void Dispose()
		{
			this.FreeAll();
			this.m_tofree.Dispose();
			this.m_allocated.Dispose();
		}

		public AllocatorManager.TryFunction Function
		{
			get
			{
				return new AllocatorManager.TryFunction(AutoFreeAllocator.Try);
			}
		}

		public unsafe int Try(ref AllocatorManager.Block block)
		{
			if (block.Range.Pointer == IntPtr.Zero)
			{
				if (block.Bytes == 0L)
				{
					return 0;
				}
				byte* ptr = (byte*)Memory.Unmanaged.Allocate(block.Bytes, block.Alignment, this.m_backingAllocatorHandle);
				block.Range.Pointer = (IntPtr)((void*)ptr);
				block.AllocatedItems = block.Range.Items;
				this.m_allocated.LockfreeAdd(block.Range.Pointer);
				return 0;
			}
			else
			{
				if (block.Range.Items == 0)
				{
					this.m_tofree.LockfreeAdd(block.Range.Pointer);
					block.Range.Pointer = IntPtr.Zero;
					block.AllocatedItems = 0;
					return 0;
				}
				return -1;
			}
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
		internal static int Try(IntPtr state, ref AllocatorManager.Block block)
		{
			return AutoFreeAllocator.Try_000000E3$BurstDirectCall.Invoke(state, ref block);
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

		[BurstCompile]
		[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int Try$BurstManaged(IntPtr state, ref AllocatorManager.Block block)
		{
			return ((AutoFreeAllocator*)(void*)state)->Try(ref block);
		}

		private ArrayOfArrays<IntPtr> m_allocated;

		private ArrayOfArrays<IntPtr> m_tofree;

		private AllocatorManager.AllocatorHandle m_handle;

		private AllocatorManager.AllocatorHandle m_backingAllocatorHandle;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate int Try_000000E3$PostfixBurstDelegate(IntPtr state, ref AllocatorManager.Block block);

		internal static class Try_000000E3$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (AutoFreeAllocator.Try_000000E3$BurstDirectCall.Pointer == 0)
				{
					AutoFreeAllocator.Try_000000E3$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<AutoFreeAllocator.Try_000000E3$PostfixBurstDelegate>(new AutoFreeAllocator.Try_000000E3$PostfixBurstDelegate(AutoFreeAllocator.Try)).Value;
				}
				A_0 = AutoFreeAllocator.Try_000000E3$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				AutoFreeAllocator.Try_000000E3$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			public static int Invoke(IntPtr state, ref AllocatorManager.Block block)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = AutoFreeAllocator.Try_000000E3$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(System.IntPtr,Unity.Collections.AllocatorManager/Block&), state, ref block, functionPointer);
					}
				}
				return AutoFreeAllocator.Try$BurstManaged(state, ref block);
			}

			private static IntPtr Pointer;
		}
	}
}
