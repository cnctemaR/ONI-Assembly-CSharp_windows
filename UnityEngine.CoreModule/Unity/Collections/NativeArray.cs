using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Internal;

namespace Unity.Collections
{
	[NativeContainerSupportsDeallocateOnJobCompletion]
	[DebuggerTypeProxy(typeof(NativeArrayDebugView<>))]
	[NativeContainerSupportsDeferredConvertListToArray]
	[DebuggerDisplay("Length = {Length}")]
	[NativeContainerSupportsMinMaxWriteRestriction]
	[NativeContainer]
	public struct NativeArray<T> : IDisposable, IEnumerable<T>, IEquatable<NativeArray<T>>, IEnumerable where T : struct
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
			NativeArray<T>.Copy(array, this);
		}

		public NativeArray(NativeArray<T> array, Allocator allocator)
		{
			NativeArray<T>.Allocate(array.Length, allocator, out this);
			NativeArray<T>.Copy(array, this);
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
			[CompilerGenerated]
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
				throw new InvalidOperationException(string.Format("{0} used in NativeArray<{1}> must be blittable.\n{2}", typeof(T), typeof(T), UnsafeUtility.GetReasonForValueTypeNonBlittable<T>()));
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
			[CompilerGenerated]
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
			NativeArray<T>.Copy(array, this);
		}

		[WriteAccessRequired]
		public void CopyFrom(NativeArray<T> array)
		{
			NativeArray<T>.Copy(array, this);
		}

		public void CopyTo(T[] array)
		{
			NativeArray<T>.Copy(this, array);
		}

		public void CopyTo(NativeArray<T> array)
		{
			NativeArray<T>.Copy(this, array);
		}

		public T[] ToArray()
		{
			T[] array = new T[this.Length];
			NativeArray<T>.Copy(this, array, this.Length);
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

		public bool Equals(NativeArray<T> other)
		{
			return this.m_Buffer == other.m_Buffer && this.m_Length == other.m_Length;
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && obj is NativeArray<T> && this.Equals((NativeArray<T>)obj);
		}

		public override int GetHashCode()
		{
			return (this.m_Buffer * 397) ^ this.m_Length;
		}

		public static bool operator ==(NativeArray<T> left, NativeArray<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(NativeArray<T> left, NativeArray<T> right)
		{
			return !left.Equals(right);
		}

		public static void Copy(NativeArray<T> src, NativeArray<T> dst)
		{
			NativeArray<T>.Copy(src, 0, dst, 0, src.Length);
		}

		public static void Copy(T[] src, NativeArray<T> dst)
		{
			NativeArray<T>.Copy(src, 0, dst, 0, src.Length);
		}

		public static void Copy(NativeArray<T> src, T[] dst)
		{
			NativeArray<T>.Copy(src, 0, dst, 0, src.Length);
		}

		public static void Copy(NativeArray<T> src, NativeArray<T> dst, int length)
		{
			NativeArray<T>.Copy(src, 0, dst, 0, length);
		}

		public static void Copy(T[] src, NativeArray<T> dst, int length)
		{
			NativeArray<T>.Copy(src, 0, dst, 0, length);
		}

		public static void Copy(NativeArray<T> src, T[] dst, int length)
		{
			NativeArray<T>.Copy(src, 0, dst, 0, length);
		}

		public unsafe static void Copy(NativeArray<T> src, int srcIndex, NativeArray<T> dst, int dstIndex, int length)
		{
			UnsafeUtility.MemCpy((void*)((byte*)dst.m_Buffer + dstIndex * UnsafeUtility.SizeOf<T>()), (void*)((byte*)src.m_Buffer + srcIndex * UnsafeUtility.SizeOf<T>()), (long)(length * UnsafeUtility.SizeOf<T>()));
		}

		public unsafe static void Copy(T[] src, int srcIndex, NativeArray<T> dst, int dstIndex, int length)
		{
			GCHandle gchandle = GCHandle.Alloc(src, GCHandleType.Pinned);
			IntPtr intPtr = gchandle.AddrOfPinnedObject();
			UnsafeUtility.MemCpy((void*)((byte*)dst.m_Buffer + dstIndex * UnsafeUtility.SizeOf<T>()), (void*)((byte*)(void*)intPtr + srcIndex * UnsafeUtility.SizeOf<T>()), (long)(length * UnsafeUtility.SizeOf<T>()));
			gchandle.Free();
		}

		public unsafe static void Copy(NativeArray<T> src, int srcIndex, T[] dst, int dstIndex, int length)
		{
			GCHandle gchandle = GCHandle.Alloc(dst, GCHandleType.Pinned);
			IntPtr intPtr = gchandle.AddrOfPinnedObject();
			UnsafeUtility.MemCpy((void*)((byte*)(void*)intPtr + dstIndex * UnsafeUtility.SizeOf<T>()), (void*)((byte*)src.m_Buffer + srcIndex * UnsafeUtility.SizeOf<T>()), (long)(length * UnsafeUtility.SizeOf<T>()));
			gchandle.Free();
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
				[CompilerGenerated]
				get
				{
					return this.m_Array[this.m_Index];
				}
			}

			object IEnumerator.Current
			{
				[CompilerGenerated]
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
