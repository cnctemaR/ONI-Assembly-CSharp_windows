using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityEngine.Collections
{
	[NativeContainer]
	[DebuggerDisplay("Length = {Length}")]
	[DebuggerTypeProxy(typeof(NativeArrayDebugView<>))]
	[NativeContainerSupportsMinMaxWriteRestriction]
	public struct NativeArray<T> : IDisposable, IEnumerable<T>, IEnumerable where T : struct
	{
		public NativeArray(int length, Allocator allocMode)
		{
			NativeArray<T>.Allocate(length, allocMode, out this);
		}

		public NativeArray(T[] array, Allocator allocMode)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			NativeArray<T>.Allocate(array.Length, allocMode, out this);
			this.FromArray(array);
		}

		internal NativeArray(IntPtr dataPointer, int length)
		{
			this = new NativeArray<T>(dataPointer, length, Allocator.None);
		}

		internal NativeArray(IntPtr dataPointer, int length, int stride, AtomicSafetyHandle safety, Allocator allocMode)
		{
			this.m_Buffer = dataPointer;
			this.m_Length = length;
			this.m_Stride = stride;
			this.m_AllocatorLabel = allocMode;
		}

		private NativeArray(IntPtr dataPointer, int length, Allocator allocMode)
		{
			if (dataPointer == IntPtr.Zero)
			{
				throw new ArgumentOutOfRangeException("dataPointer", "Pointer must not be zero");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Length must be >= 0");
			}
			this.m_Buffer = dataPointer;
			this.m_Length = length;
			this.m_Stride = UnsafeUtility.SizeOf<T>();
			this.m_AllocatorLabel = allocMode;
		}

		public int Length
		{
			get
			{
				return this.m_Length;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index >= this.m_Length)
				{
					this.FailOutOfRangeError(index);
				}
				return UnsafeUtility.ReadArrayElement<T>(this.m_Buffer, index * this.m_Stride);
			}
			set
			{
				if (index >= this.m_Length)
				{
					this.FailOutOfRangeError(index);
				}
				UnsafeUtility.WriteArrayElement<T>(this.m_Buffer, index * this.m_Stride, value);
			}
		}

		public void Dispose()
		{
			UnsafeUtility.Free(this.m_Buffer, this.m_AllocatorLabel);
			this.m_Buffer = IntPtr.Zero;
			this.m_Length = 0;
		}

		public IntPtr GetUnsafeReadBufferPtr()
		{
			return this.m_Buffer;
		}

		public IntPtr GetUnsafeWriteBufferPtr()
		{
			return this.m_Buffer;
		}

		public void FromArray(T[] array)
		{
			if (this.Length != array.Length)
			{
				throw new ArgumentException("Array length does not match the length of this instance");
			}
			for (int i = 0; i < this.Length; i++)
			{
				UnsafeUtility.WriteArrayElement<T>(this.m_Buffer, i * this.m_Stride, array[i]);
			}
		}

		public T[] ToArray()
		{
			T[] array = new T[this.Length];
			for (int i = 0; i < this.Length; i++)
			{
				array[i] = UnsafeUtility.ReadArrayElement<T>(this.m_Buffer, i * this.m_Stride);
			}
			return array;
		}

		private void FailOutOfRangeError(int index)
		{
			throw new IndexOutOfRangeException(string.Format("Index {0} is out of range of '{1}' Length.", index, this.Length));
		}

		private static void Allocate(int length, Allocator allocMode, out NativeArray<T> outArray)
		{
			if (allocMode <= Allocator.None)
			{
				throw new ArgumentOutOfRangeException("allocMode", "Allocator must be Temp, Job or Persistent");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Length must be >= 0");
			}
			long num = (long)UnsafeUtility.SizeOf<T>() * (long)length;
			if (num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("length", "Length * sizeof(T) cannot exceed " + int.MaxValue + "bytes");
			}
			outArray = new NativeArray<T>(UnsafeUtility.Malloc((int)num, UnsafeUtility.AlignOf<T>(), allocMode), length, allocMode);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new NativeArray<T>.Enumerator(ref this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private IntPtr m_Buffer;

		private int m_Length;

		private int m_Stride;

		private readonly Allocator m_AllocatorLabel;

		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			public Enumerator(ref NativeArray<T> array)
			{
				this.array = array;
				this.index = -1;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				this.index++;
				return this.index < this.array.Length;
			}

			public void Reset()
			{
				this.index = -1;
			}

			public T Current
			{
				get
				{
					return this.array[this.index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private NativeArray<T> array;

			private int index;
		}
	}
}
