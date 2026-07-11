using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System
{
	[DebuggerTypeProxy(typeof(SpanDebugView<>))]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly ref struct Span<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span(T[] array)
		{
			if (array == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			if (default(T) == null && array.GetType() != typeof(T[]))
			{
				ThrowHelper.ThrowArrayTypeMismatchException_ArrayTypeMustBeExactMatch(typeof(T));
			}
			this._length = array.Length;
			this._pinnable = Unsafe.As<Pinnable<T>>(array);
			this._byteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span(T[] array, int start, int length)
		{
			if (array == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			if (default(T) == null && array.GetType() != typeof(T[]))
			{
				ThrowHelper.ThrowArrayTypeMismatchException_ArrayTypeMustBeExactMatch(typeof(T));
			}
			if (start > array.Length || length > array.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			this._length = length;
			this._pinnable = Unsafe.As<Pinnable<T>>(array);
			this._byteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment.Add<T>(start);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe Span(void* pointer, int length)
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			if (length < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			this._length = length;
			this._pinnable = null;
			this._byteOffset = new IntPtr(pointer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> DangerousCreate(object obj, ref T objectData, int length)
		{
			Pinnable<T> pinnable = Unsafe.As<Pinnable<T>>(obj);
			IntPtr intPtr = Unsafe.ByteOffset<T>(ref pinnable.Data, ref objectData);
			return new Span<T>(pinnable, intPtr, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Span(Pinnable<T> pinnable, IntPtr byteOffset, int length)
		{
			this._length = length;
			this._pinnable = pinnable;
			this._byteOffset = byteOffset;
		}

		private string DebuggerDisplay
		{
			get
			{
				return string.Format("{{{0}[{1}]}}", typeof(T).Name, this._length);
			}
		}

		public int Length
		{
			get
			{
				return this._length;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this._length == 0;
			}
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (index >= this._length)
				{
					ThrowHelper.ThrowIndexOutOfRangeException();
				}
				if (this._pinnable == null)
				{
					return Unsafe.Add<T>(Unsafe.AsRef<T>(this._byteOffset.ToPointer()), index);
				}
				return Unsafe.Add<T>(Unsafe.AddByteOffset<T>(ref this._pinnable.Data, this._byteOffset), index);
			}
		}

		public unsafe void Clear()
		{
			int length = this._length;
			if (length == 0)
			{
				return;
			}
			UIntPtr uintPtr = (UIntPtr)((ulong)length * (ulong)((long)Unsafe.SizeOf<T>()));
			if ((Unsafe.SizeOf<T>() & (sizeof(IntPtr) - 1)) != 0)
			{
				if (this._pinnable == null)
				{
					byte* ptr = (byte*)this._byteOffset.ToPointer();
					SpanHelpers.ClearLessThanPointerSized(ptr, uintPtr);
					return;
				}
				SpanHelpers.ClearLessThanPointerSized(Unsafe.As<T, byte>(Unsafe.AddByteOffset<T>(ref this._pinnable.Data, this._byteOffset)), uintPtr);
				return;
			}
			else
			{
				if (SpanHelpers.IsReferenceOrContainsReferences<T>())
				{
					UIntPtr uintPtr2 = (UIntPtr)((ulong)((long)(length * Unsafe.SizeOf<T>() / sizeof(IntPtr))));
					SpanHelpers.ClearPointerSizedWithReferences(Unsafe.As<T, IntPtr>(this.DangerousGetPinnableReference()), uintPtr2);
					return;
				}
				SpanHelpers.ClearPointerSizedWithoutReferences(Unsafe.As<T, byte>(this.DangerousGetPinnableReference()), uintPtr);
				return;
			}
		}

		public unsafe void Fill(T value)
		{
			int length = this._length;
			if (length == 0)
			{
				return;
			}
			if (Unsafe.SizeOf<T>() != 1)
			{
				ref T ptr = ref this.DangerousGetPinnableReference();
				int i;
				for (i = 0; i < (length & -8); i += 8)
				{
					*Unsafe.Add<T>(ref ptr, i) = value;
					*Unsafe.Add<T>(ref ptr, i + 1) = value;
					*Unsafe.Add<T>(ref ptr, i + 2) = value;
					*Unsafe.Add<T>(ref ptr, i + 3) = value;
					*Unsafe.Add<T>(ref ptr, i + 4) = value;
					*Unsafe.Add<T>(ref ptr, i + 5) = value;
					*Unsafe.Add<T>(ref ptr, i + 6) = value;
					*Unsafe.Add<T>(ref ptr, i + 7) = value;
				}
				if (i < (length & -4))
				{
					*Unsafe.Add<T>(ref ptr, i) = value;
					*Unsafe.Add<T>(ref ptr, i + 1) = value;
					*Unsafe.Add<T>(ref ptr, i + 2) = value;
					*Unsafe.Add<T>(ref ptr, i + 3) = value;
					i += 4;
				}
				while (i < length)
				{
					*Unsafe.Add<T>(ref ptr, i) = value;
					i++;
				}
				return;
			}
			byte b = *Unsafe.As<T, byte>(ref value);
			if (this._pinnable == null)
			{
				Unsafe.InitBlockUnaligned(this._byteOffset.ToPointer(), b, (uint)length);
				return;
			}
			Unsafe.InitBlockUnaligned(Unsafe.As<T, byte>(Unsafe.AddByteOffset<T>(ref this._pinnable.Data, this._byteOffset)), b, (uint)length);
		}

		public void CopyTo(Span<T> destination)
		{
			if (!this.TryCopyTo(destination))
			{
				ThrowHelper.ThrowArgumentException_DestinationTooShort();
			}
		}

		public bool TryCopyTo(Span<T> destination)
		{
			int length = this._length;
			int length2 = destination._length;
			if (length == 0)
			{
				return true;
			}
			if (length > length2)
			{
				return false;
			}
			ref T ptr = ref this.DangerousGetPinnableReference();
			SpanHelpers.CopyTo<T>(destination.DangerousGetPinnableReference(), length2, ref ptr, length);
			return true;
		}

		public static bool operator ==(Span<T> left, Span<T> right)
		{
			return left._length == right._length && Unsafe.AreSame<T>(left.DangerousGetPinnableReference(), right.DangerousGetPinnableReference());
		}

		public static bool operator !=(Span<T> left, Span<T> right)
		{
			return !(left == right);
		}

		[Obsolete("Equals() on Span will always throw an exception. Use == instead.")]
		public override bool Equals(object obj)
		{
			throw new NotSupportedException("Equals() on Span and ReadOnlySpan is not supported. Use operator== instead.");
		}

		[Obsolete("GetHashCode() on Span will always throw an exception.")]
		public override int GetHashCode()
		{
			throw new NotSupportedException("GetHashCode() on Span and ReadOnlySpan is not supported.");
		}

		public static implicit operator Span<T>(T[] array)
		{
			return new Span<T>(array);
		}

		public static implicit operator Span<T>(ArraySegment<T> arraySegment)
		{
			return new Span<T>(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}

		public static implicit operator ReadOnlySpan<T>(Span<T> span)
		{
			return new ReadOnlySpan<T>(span._pinnable, span._byteOffset, span._length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<T> Slice(int start)
		{
			if (start > this._length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			IntPtr intPtr = this._byteOffset.Add<T>(start);
			int num = this._length - start;
			return new Span<T>(this._pinnable, intPtr, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<T> Slice(int start, int length)
		{
			if (start > this._length || length > this._length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			IntPtr intPtr = this._byteOffset.Add<T>(start);
			return new Span<T>(this._pinnable, intPtr, length);
		}

		public T[] ToArray()
		{
			if (this._length == 0)
			{
				return SpanHelpers.PerTypeValues<T>.EmptyArray;
			}
			T[] array = new T[this._length];
			this.CopyTo(array);
			return array;
		}

		public static Span<T> Empty
		{
			get
			{
				return default(Span<T>);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T DangerousGetPinnableReference()
		{
			if (this._pinnable == null)
			{
				return Unsafe.AsRef<T>(this._byteOffset.ToPointer());
			}
			return Unsafe.AddByteOffset<T>(ref this._pinnable.Data, this._byteOffset);
		}

		internal Pinnable<T> Pinnable
		{
			get
			{
				return this._pinnable;
			}
		}

		internal IntPtr ByteOffset
		{
			get
			{
				return this._byteOffset;
			}
		}

		private readonly Pinnable<T> _pinnable;

		private readonly IntPtr _byteOffset;

		private readonly int _length;
	}
}
