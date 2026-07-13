using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Internal;

namespace Unity.Collections
{
	[DebuggerTypeProxy(typeof(NativeArrayDebugView<>))]
	[DebuggerDisplay("Length = {m_Length}")]
	[NativeContainerSupportsDeallocateOnJobCompletion]
	[NativeContainerSupportsMinMaxWriteRestriction]
	[NativeContainerSupportsDeferredConvertListToArray]
	[NativeContainer]
	public struct NativeArray<T> : IDisposable, IEnumerable<T>, IEnumerable, IEquatable<NativeArray<T>> where T : struct
	{
		public NativeArray(int length, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			NativeArray<T>.Allocate(length, allocator, out this);
			bool flag = (options & NativeArrayOptions.ClearMemory) == NativeArrayOptions.ClearMemory;
			if (flag)
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
			NativeArray<T>.Copy(array, 0, this, 0, array.Length);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckAllocateArguments(int length, Allocator allocator)
		{
			bool flag = allocator <= Allocator.None;
			if (flag)
			{
				throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", "allocator");
			}
			bool flag2 = allocator >= Allocator.FirstUserIndex;
			if (flag2)
			{
				throw new ArgumentException("Use CollectionHelper.CreateNativeArray for custom allocator", "allocator");
			}
			bool flag3 = length < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("length", "Length must be >= 0");
			}
		}

		private static void Allocate(int length, Allocator allocator, out NativeArray<T> array)
		{
			long num = (long)UnsafeUtility.SizeOf<T>() * (long)length;
			array = default(NativeArray<T>);
			array.m_Buffer = UnsafeUtility.MallocTracked(num, UnsafeUtility.AlignOf<T>(), allocator, 0);
			array.m_Length = length;
			array.m_AllocatorLabel = allocator;
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Length;
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[BurstDiscard]
		internal static void IsUnmanagedAndThrow()
		{
			bool flag = !UnsafeUtility.IsUnmanaged<T>();
			if (flag)
			{
				throw new InvalidOperationException(string.Format("{0} used in NativeArray<{1}> must be unmanaged (contain no managed types).", typeof(T), typeof(T)));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckElementReadAccess(int index)
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckElementWriteAccess(int index)
		{
		}

		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return UnsafeUtility.ReadArrayElement<T>(this.m_Buffer, index);
			}
			[WriteAccessRequired]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				UnsafeUtility.WriteArrayElement<T>(this.m_Buffer, index, value);
			}
		}

		public bool IsCreated
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Buffer != null;
			}
		}

		[WriteAccessRequired]
		public void Dispose()
		{
			bool flag = !this.IsCreated;
			if (!flag)
			{
				bool flag2 = this.m_AllocatorLabel == Allocator.Invalid;
				if (flag2)
				{
					throw new InvalidOperationException("The NativeArray can not be Disposed because it was not allocated with a valid allocator.");
				}
				bool flag3 = this.m_AllocatorLabel > Allocator.None;
				if (flag3)
				{
					UnsafeUtility.FreeTracked(this.m_Buffer, this.m_AllocatorLabel);
					this.m_AllocatorLabel = Allocator.Invalid;
				}
				this.m_Buffer = null;
			}
		}

		public JobHandle Dispose(JobHandle inputDeps)
		{
			bool flag = !this.IsCreated;
			JobHandle jobHandle;
			if (flag)
			{
				jobHandle = inputDeps;
			}
			else
			{
				bool flag2 = this.m_AllocatorLabel >= Allocator.FirstUserIndex;
				if (flag2)
				{
					throw new InvalidOperationException("The NativeArray can not be Disposed because it was allocated with a custom allocator, use CollectionHelper.Dispose in com.unity.collections package.");
				}
				bool flag3 = this.m_AllocatorLabel > Allocator.None;
				if (flag3)
				{
					JobHandle jobHandle2 = new NativeArrayDisposeJob
					{
						Data = new NativeArrayDispose
						{
							m_Buffer = this.m_Buffer,
							m_AllocatorLabel = this.m_AllocatorLabel
						}
					}.Schedule(inputDeps);
					this.m_Buffer = null;
					this.m_AllocatorLabel = Allocator.Invalid;
					jobHandle = jobHandle2;
				}
				else
				{
					this.m_Buffer = null;
					jobHandle = inputDeps;
				}
			}
			return jobHandle;
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
			bool flag = obj == null;
			return !flag && obj is NativeArray<T> && this.Equals((NativeArray<T>)obj);
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
			NativeArray<T>.CopySafe(src, 0, dst, 0, src.Length);
		}

		public static void Copy(NativeArray<T>.ReadOnly src, NativeArray<T> dst)
		{
			NativeArray<T>.CopySafe(src, 0, dst, 0, src.Length);
		}

		public static void Copy(T[] src, NativeArray<T> dst)
		{
			NativeArray<T>.CopySafe(src, 0, dst, 0, src.Length);
		}

		public static void Copy(NativeArray<T> src, T[] dst)
		{
			NativeArray<T>.CopySafe(src, 0, dst, 0, src.Length);
		}

		public static void Copy(NativeArray<T>.ReadOnly src, T[] dst)
		{
			NativeArray<T>.CopySafe(src, 0, dst, 0, src.Length);
		}

		public static void Copy(NativeArray<T> src, NativeArray<T> dst, int length)
		{
			NativeArray<T>.CopySafe(src, 0, dst, 0, length);
		}

		public static void Copy(NativeArray<T>.ReadOnly src, NativeArray<T> dst, int length)
		{
			NativeArray<T>.CopySafe(src, 0, dst, 0, length);
		}

		public static void Copy(T[] src, NativeArray<T> dst, int length)
		{
			NativeArray<T>.CopySafe(src, 0, dst, 0, length);
		}

		public static void Copy(NativeArray<T> src, T[] dst, int length)
		{
			NativeArray<T>.CopySafe(src, 0, dst, 0, length);
		}

		public static void Copy(NativeArray<T>.ReadOnly src, T[] dst, int length)
		{
			NativeArray<T>.CopySafe(src, 0, dst, 0, length);
		}

		public static void Copy(NativeArray<T> src, int srcIndex, NativeArray<T> dst, int dstIndex, int length)
		{
			NativeArray<T>.CopySafe(src, srcIndex, dst, dstIndex, length);
		}

		public static void Copy(NativeArray<T>.ReadOnly src, int srcIndex, NativeArray<T> dst, int dstIndex, int length)
		{
			NativeArray<T>.CopySafe(src, srcIndex, dst, dstIndex, length);
		}

		public static void Copy(T[] src, int srcIndex, NativeArray<T> dst, int dstIndex, int length)
		{
			NativeArray<T>.CopySafe(src, srcIndex, dst, dstIndex, length);
		}

		public static void Copy(NativeArray<T> src, int srcIndex, T[] dst, int dstIndex, int length)
		{
			NativeArray<T>.CopySafe(src, srcIndex, dst, dstIndex, length);
		}

		public static void Copy(NativeArray<T>.ReadOnly src, int srcIndex, T[] dst, int dstIndex, int length)
		{
			NativeArray<T>.CopySafe(src, srcIndex, dst, dstIndex, length);
		}

		private unsafe static void CopySafe(NativeArray<T> src, int srcIndex, NativeArray<T> dst, int dstIndex, int length)
		{
			UnsafeUtility.MemCpy((void*)((byte*)dst.m_Buffer + dstIndex * UnsafeUtility.SizeOf<T>()), (void*)((byte*)src.m_Buffer + srcIndex * UnsafeUtility.SizeOf<T>()), (long)(length * UnsafeUtility.SizeOf<T>()));
		}

		private unsafe static void CopySafe(NativeArray<T>.ReadOnly src, int srcIndex, NativeArray<T> dst, int dstIndex, int length)
		{
			UnsafeUtility.MemCpy((void*)((byte*)dst.m_Buffer + dstIndex * UnsafeUtility.SizeOf<T>()), (void*)((byte*)src.m_Buffer + srcIndex * UnsafeUtility.SizeOf<T>()), (long)(length * UnsafeUtility.SizeOf<T>()));
		}

		private unsafe static void CopySafe(T[] src, int srcIndex, NativeArray<T> dst, int dstIndex, int length)
		{
			GCHandle gchandle = GCHandle.Alloc(src, GCHandleType.Pinned);
			IntPtr intPtr = gchandle.AddrOfPinnedObject();
			UnsafeUtility.MemCpy((void*)((byte*)dst.m_Buffer + dstIndex * UnsafeUtility.SizeOf<T>()), (void*)((byte*)(void*)intPtr + srcIndex * UnsafeUtility.SizeOf<T>()), (long)(length * UnsafeUtility.SizeOf<T>()));
			gchandle.Free();
		}

		private unsafe static void CopySafe(NativeArray<T> src, int srcIndex, T[] dst, int dstIndex, int length)
		{
			GCHandle gchandle = GCHandle.Alloc(dst, GCHandleType.Pinned);
			IntPtr intPtr = gchandle.AddrOfPinnedObject();
			UnsafeUtility.MemCpy((void*)((byte*)(void*)intPtr + dstIndex * UnsafeUtility.SizeOf<T>()), (void*)((byte*)src.m_Buffer + srcIndex * UnsafeUtility.SizeOf<T>()), (long)(length * UnsafeUtility.SizeOf<T>()));
			gchandle.Free();
		}

		private unsafe static void CopySafe(NativeArray<T>.ReadOnly src, int srcIndex, T[] dst, int dstIndex, int length)
		{
			GCHandle gchandle = GCHandle.Alloc(dst, GCHandleType.Pinned);
			IntPtr intPtr = gchandle.AddrOfPinnedObject();
			UnsafeUtility.MemCpy((void*)((byte*)(void*)intPtr + dstIndex * UnsafeUtility.SizeOf<T>()), (void*)((byte*)src.m_Buffer + srcIndex * UnsafeUtility.SizeOf<T>()), (long)(length * UnsafeUtility.SizeOf<T>()));
			gchandle.Free();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckCopyPtr(T[] ptr)
		{
			bool flag = ptr == null;
			if (flag)
			{
				throw new ArgumentNullException("ptr");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckCopyLengths(int srcLength, int dstLength)
		{
			bool flag = srcLength != dstLength;
			if (flag)
			{
				throw new ArgumentException("source and destination length must be the same");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckCopyArguments(int srcLength, int srcIndex, int dstLength, int dstIndex, int length)
		{
			bool flag = length < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("length", "length must be equal or greater than zero.");
			}
			bool flag2 = srcIndex < 0 || srcIndex > srcLength || (srcIndex == srcLength && srcLength > 0);
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("srcIndex", "srcIndex is outside the range of valid indexes for the source NativeArray.");
			}
			bool flag3 = dstIndex < 0 || dstIndex > dstLength || (dstIndex == dstLength && dstLength > 0);
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("dstIndex", "dstIndex is outside the range of valid indexes for the destination NativeArray.");
			}
			bool flag4 = srcIndex + length > srcLength;
			if (flag4)
			{
				throw new ArgumentException("length is greater than the number of elements from srcIndex to the end of the source NativeArray.", "length");
			}
			bool flag5 = srcIndex + length < 0;
			if (flag5)
			{
				throw new ArgumentException("srcIndex + length causes an integer overflow");
			}
			bool flag6 = dstIndex + length > dstLength;
			if (flag6)
			{
				throw new ArgumentException("length is greater than the number of elements from dstIndex to the end of the destination NativeArray.", "length");
			}
			bool flag7 = dstIndex + length < 0;
			if (flag7)
			{
				throw new ArgumentException("dstIndex + length causes an integer overflow");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckReinterpretLoadRange<U>(int sourceIndex) where U : struct
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckReinterpretStoreRange<U>(int destIndex) where U : struct
		{
		}

		public unsafe U ReinterpretLoad<U>(int sourceIndex) where U : struct
		{
			byte* ptr = (byte*)this.m_Buffer + (long)UnsafeUtility.SizeOf<T>() * (long)sourceIndex;
			return UnsafeUtility.ReadArrayElement<U>((void*)ptr, 0);
		}

		public unsafe void ReinterpretStore<U>(int destIndex, U data) where U : struct
		{
			byte* ptr = (byte*)this.m_Buffer + (long)UnsafeUtility.SizeOf<T>() * (long)destIndex;
			UnsafeUtility.WriteArrayElement<U>((void*)ptr, 0, data);
		}

		private NativeArray<U> InternalReinterpret<U>(int length) where U : struct
		{
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<U>(this.m_Buffer, length, this.m_AllocatorLabel);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckReinterpretSize<U>() where U : struct
		{
			bool flag = UnsafeUtility.SizeOf<T>() != UnsafeUtility.SizeOf<U>();
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Types {0} and {1} are different sizes - direct reinterpretation is not possible. If this is what you intended, use Reinterpret(<type size>)", typeof(T), typeof(U)));
			}
		}

		public NativeArray<U> Reinterpret<U>() where U : struct
		{
			return this.InternalReinterpret<U>(this.Length);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckReinterpretSize<U>(long tSize, long uSize, int expectedTypeSize, long byteLen, long uLen)
		{
			bool flag = tSize != (long)expectedTypeSize;
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Type {0} was expected to be {1} but is {2} bytes", typeof(T), expectedTypeSize, tSize));
			}
			bool flag2 = uLen * uSize != byteLen;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Types {0} (array length {1}) and {2} cannot be aliased due to size constraints. The size of the types and lengths involved must line up.", typeof(T), this.Length, typeof(U)));
			}
		}

		public NativeArray<U> Reinterpret<U>(int expectedTypeSize) where U : struct
		{
			long num = (long)UnsafeUtility.SizeOf<T>();
			long num2 = (long)UnsafeUtility.SizeOf<U>();
			long num3 = (long)this.Length * num;
			long num4 = num3 / num2;
			return this.InternalReinterpret<U>((int)num4);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckGetSubArrayArguments(int start, int length)
		{
			bool flag = start < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("start", "start must be >= 0");
			}
			bool flag2 = start + length > this.Length;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("length", string.Format("sub array range {0}-{1} is outside the range of the native array 0-{2}", start, start + length - 1, this.Length - 1));
			}
			bool flag3 = start + length < 0;
			if (flag3)
			{
				throw new ArgumentException(string.Format("sub array range {0}-{1} caused an integer overflow and is outside the range of the native array 0-{2}", start, start + length - 1, this.Length - 1));
			}
		}

		public unsafe NativeArray<T> GetSubArray(int start, int length)
		{
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)((byte*)this.m_Buffer + (long)UnsafeUtility.SizeOf<T>() * (long)start), length, Allocator.None);
		}

		public NativeArray<T>.ReadOnly AsReadOnly()
		{
			return new NativeArray<T>.ReadOnly(this.m_Buffer, this.m_Length);
		}

		[WriteAccessRequired]
		public readonly Span<T> AsSpan()
		{
			return new Span<T>(this.m_Buffer, this.m_Length);
		}

		public readonly ReadOnlySpan<T> AsReadOnlySpan()
		{
			return new ReadOnlySpan<T>(this.m_Buffer, this.m_Length);
		}

		public static implicit operator Span<T>(in NativeArray<T> source)
		{
			return source.AsSpan();
		}

		public static implicit operator ReadOnlySpan<T>(in NativeArray<T> source)
		{
			return source.AsReadOnlySpan();
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
				this.value = default(T);
			}

			public void Dispose()
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				this.m_Index++;
				bool flag = this.m_Index < this.m_Array.m_Length;
				bool flag2;
				if (flag)
				{
					this.value = UnsafeUtility.ReadArrayElement<T>(this.m_Array.m_Buffer, this.m_Index);
					flag2 = true;
				}
				else
				{
					this.value = default(T);
					flag2 = false;
				}
				return flag2;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			public T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.value;
				}
			}

			object IEnumerator.Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.Current;
				}
			}

			private NativeArray<T> m_Array;

			private int m_Index;

			private T value;
		}

		[NativeContainerIsReadOnly]
		[DebuggerDisplay("Length = {Length}")]
		[DebuggerTypeProxy(typeof(NativeArrayReadOnlyDebugView<>))]
		[NativeContainer]
		public struct ReadOnly : IEnumerable<T>, IEnumerable
		{
			internal unsafe ReadOnly(void* buffer, int length)
			{
				this.m_Buffer = buffer;
				this.m_Length = length;
			}

			public int Length
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Length;
				}
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
				T[] array = new T[this.m_Length];
				NativeArray<T>.Copy(this, array, this.m_Length);
				return array;
			}

			public NativeArray<U>.ReadOnly Reinterpret<U>() where U : struct
			{
				return new NativeArray<U>.ReadOnly(this.m_Buffer, this.m_Length);
			}

			public T this[int index]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return UnsafeUtility.ReadArrayElement<T>(this.m_Buffer, index);
				}
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void CheckElementReadAccess(int index)
			{
				bool flag = index >= this.m_Length;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("Index {0} is out of range (must be between 0 and {1}).", index, this.m_Length - 1));
				}
			}

			public bool IsCreated
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.m_Buffer != null;
				}
			}

			public NativeArray<T>.ReadOnly.Enumerator GetEnumerator()
			{
				return new NativeArray<T>.ReadOnly.Enumerator(in this);
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public readonly ReadOnlySpan<T> AsReadOnlySpan()
			{
				return new ReadOnlySpan<T>(this.m_Buffer, this.m_Length);
			}

			public static implicit operator ReadOnlySpan<T>(in NativeArray<T>.ReadOnly source)
			{
				return source.AsReadOnlySpan();
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe void* m_Buffer;

			internal int m_Length;

			[ExcludeFromDocs]
			public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
			{
				public Enumerator(in NativeArray<T>.ReadOnly array)
				{
					this.m_Array = array;
					this.m_Index = -1;
					this.value = default(T);
				}

				public void Dispose()
				{
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool MoveNext()
				{
					this.m_Index++;
					bool flag = this.m_Index < this.m_Array.m_Length;
					bool flag2;
					if (flag)
					{
						this.value = UnsafeUtility.ReadArrayElement<T>(this.m_Array.m_Buffer, this.m_Index);
						flag2 = true;
					}
					else
					{
						this.value = default(T);
						flag2 = false;
					}
					return flag2;
				}

				public void Reset()
				{
					this.m_Index = -1;
				}

				public T Current
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get
					{
						return this.value;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return this.Current;
					}
				}

				private NativeArray<T>.ReadOnly m_Array;

				private int m_Index;

				private T value;
			}
		}
	}
}
