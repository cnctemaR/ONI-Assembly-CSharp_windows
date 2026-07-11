using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Internal;

namespace Unity.Collections
{
	/// <summary>
	///   <para>A NativeArray exposes a buffer of native memory to managed code, making it possible to share data between managed and native.</para>
	/// </summary>
	[NativeContainer]
	[NativeContainerSupportsMinMaxWriteRestriction]
	[NativeContainerSupportsDeallocateOnJobCompletion]
	[NativeContainerSupportsDeferredConvertListToArray]
	[DebuggerTypeProxy(typeof(NativeArrayDebugView<>))]
	[DebuggerDisplay("Length = {Length}")]
	public struct NativeArray<T> : IDisposable, IEnumerable<T>, IEnumerable where T : struct
	{
		public NativeArray(int length, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			NativeArray<T>.Allocate(length, allocator, out this);
			if ((options & NativeArrayOptions.ClearMemory) == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear(this.m_Buffer, (long)this.Length * (long)UnsafeUtility.SizeOf<T>());
			}
		}

		public NativeArray(T[] array, Allocator allocator)
		{
			NativeArray<T>.Allocate(array.Length, allocator, out this);
			this.CopyFrom(array);
		}

		public NativeArray(NativeArray<T> array, Allocator allocator)
		{
			NativeArray<T>.Allocate(array.Length, allocator, out this);
			this.CopyFrom(array);
		}

		private static void Allocate(int length, Allocator allocator, out NativeArray<T> array)
		{
			long num = (long)UnsafeUtility.SizeOf<T>() * (long)length;
			array.m_Buffer = UnsafeUtility.Malloc(num, UnsafeUtility.AlignOf<T>(), allocator);
			array.m_Length = length;
			array.m_AllocatorLabel = allocator;
		}

		public int Length
		{
			get
			{
				return this.m_Length;
			}
		}

		[BurstDiscard]
		internal static void IsBlittableAndThrow()
		{
			if (!UnsafeUtility.IsBlittable<T>())
			{
				throw new ArgumentException(string.Format("{0} used in NativeArray<{0}> must be blittable", typeof(T)));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckElementReadAccess(int index)
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckElementWriteAccess(int index)
		{
		}

		public T this[int index]
		{
			get
			{
				return UnsafeUtility.ReadArrayElement<T>(this.m_Buffer, index);
			}
			[WriteAccessRequired]
			set
			{
				UnsafeUtility.WriteArrayElement<T>(this.m_Buffer, index, value);
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.m_Buffer != null;
			}
		}

		[WriteAccessRequired]
		public void Dispose()
		{
			UnsafeUtility.Free(this.m_Buffer, this.m_AllocatorLabel);
			this.m_Buffer = null;
			this.m_Length = 0;
		}

		[WriteAccessRequired]
		public void CopyFrom(T[] array)
		{
			for (int i = 0; i < this.Length; i++)
			{
				UnsafeUtility.WriteArrayElement<T>(this.m_Buffer, i, array[i]);
			}
		}

		[WriteAccessRequired]
		public void CopyFrom(NativeArray<T> array)
		{
			array.CopyTo(this);
		}

		public void CopyTo(T[] array)
		{
			for (int i = 0; i < this.Length; i++)
			{
				array[i] = UnsafeUtility.ReadArrayElement<T>(this.m_Buffer, i);
			}
		}

		public void CopyTo(NativeArray<T> array)
		{
			UnsafeUtility.MemCpy(array.m_Buffer, this.m_Buffer, (long)this.Length * (long)UnsafeUtility.SizeOf<T>());
		}

		public T[] ToArray()
		{
			T[] array = new T[this.Length];
			this.CopyTo(array);
			return array;
		}

		public NativeArray<T>.Enumerator GetEnumerator()
		{
			return new NativeArray<T>.Enumerator(ref this);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new NativeArray<T>.Enumerator(ref this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe void* m_Buffer;

		internal int m_Length;

		internal Allocator m_AllocatorLabel;

		[ExcludeFromDocs]
		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			public Enumerator(ref NativeArray<T> array)
			{
				this.m_Array = array;
				this.m_Index = -1;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				this.m_Index++;
				return this.m_Index < this.m_Array.Length;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			public T Current
			{
				get
				{
					return this.m_Array[this.m_Index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private NativeArray<T> m_Array;

			private int m_Index;
		}
	}
}
