using System;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy
{
	internal readonly struct ReadOnlyNativeVector<[global::System.Runtime.CompilerServices.IsUnmanaged] T> where T : struct, ValueType
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Count;
			}
		}

		public ReadOnlyNativeVector(IntPtr ptr, int size)
		{
			this.m_Ptr = ptr;
			this.m_Count = size;
		}

		public unsafe ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				bool flag = index < 0 || index >= this.m_Count;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (byte*)(void*)this.m_Ptr + (IntPtr)index * (IntPtr)sizeof(T);
			}
		}

		public unsafe ReadOnlySpan<T> AsReadOnlySpan()
		{
			return new ReadOnlySpan<T>((void*)this.m_Ptr, this.m_Count);
		}

		public static implicit operator ReadOnlySpan<T>(ReadOnlyNativeVector<T> vector)
		{
			return vector.AsReadOnlySpan();
		}

		private readonly IntPtr m_Ptr;

		private readonly int m_Count;
	}
}
