using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	internal struct UnmanagedArray<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : IDisposable where T : struct, ValueType
	{
		public int Length
		{
			get
			{
				return this.m_length;
			}
		}

		public unsafe UnmanagedArray(int length, AllocatorManager.AllocatorHandle allocator)
		{
			this.m_pointer = (IntPtr)((void*)Memory.Unmanaged.Array.Allocate<T>((long)length, allocator));
			this.m_length = length;
			this.m_allocator = allocator;
		}

		public unsafe void Dispose()
		{
			Memory.Unmanaged.Free<T>((T*)(void*)this.m_pointer, Allocator.Persistent);
		}

		public unsafe T* GetUnsafePointer()
		{
			return (T*)(void*)this.m_pointer;
		}

		public unsafe ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return ref *(T*)((byte*)(void*)this.m_pointer + (IntPtr)index * (IntPtr)sizeof(T));
			}
		}

		private IntPtr m_pointer;

		private int m_length;

		private AllocatorManager.AllocatorHandle m_allocator;
	}
}
