using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MonoMod;

namespace System
{
	[NullableContext(1)]
	[Nullable(0)]
	[DebuggerTypeProxy(typeof(SpanDebugView<>))]
	[DebuggerDisplay("{ToString(),raw}")]
	internal readonly ref struct ReadOnlySpan<[Nullable(2)] T>
	{
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

		public static bool operator !=([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlySpan<T> left, [Nullable(new byte[] { 0, 1 })] global::System.ReadOnlySpan<T> right)
		{
			return !(left == right);
		}

		[NullableContext(2)]
		[Obsolete("Equals() on ReadOnlySpan will always throw an exception. Use == instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			throw new NotSupportedException("Cannot call Equals on Span");
		}

		[Obsolete("GetHashCode() on ReadOnlySpan will always throw an exception.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			throw new NotSupportedException("Cannot call GetHashCodee on Span");
		}

		[return: Nullable(new byte[] { 0, 1 })]
		public static implicit operator global::System.ReadOnlySpan<T>([Nullable(new byte[] { 2, 1 })] T[] array)
		{
			return new global::System.ReadOnlySpan<T>(array);
		}

		[return: Nullable(new byte[] { 0, 1 })]
		public static implicit operator global::System.ReadOnlySpan<T>([Nullable(new byte[] { 0, 1 })] ArraySegment<T> segment)
		{
			T[] array = segment.Array;
			if (array == null)
			{
				throw new ArgumentNullException("segment");
			}
			return new global::System.ReadOnlySpan<T>(array, segment.Offset, segment.Count);
		}

		[Nullable(new byte[] { 0, 1 })]
		public static global::System.ReadOnlySpan<T> Empty
		{
			[return: Nullable(new byte[] { 0, 1 })]
			get
			{
				return default(global::System.ReadOnlySpan<T>);
			}
		}

		[NullableContext(0)]
		public global::System.ReadOnlySpan<T>.Enumerator GetEnumerator()
		{
			return new global::System.ReadOnlySpan<T>.Enumerator(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan([Nullable(new byte[] { 2, 1 })] T[] array)
		{
			if (array == null)
			{
				this = default(global::System.ReadOnlySpan<T>);
				return;
			}
			this._length = array.Length;
			this._pinnable = array;
			this._byteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan([Nullable(new byte[] { 2, 1 })] T[] array, int start, int length)
		{
			if (array == null)
			{
				if (start != 0 || length != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
				}
				this = default(global::System.ReadOnlySpan<T>);
				return;
			}
			if (start > array.Length || length > array.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			this._length = length;
			this._pinnable = array;
			this._byteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment.Add<T>(start);
		}

		[NullableContext(0)]
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ReadOnlySpan(void* pointer, int length)
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

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ReadOnlySpan(object pinnable, IntPtr byteOffset, int length)
		{
			this._length = length;
			this._pinnable = pinnable;
			this._byteOffset = byteOffset;
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
				return Unsafe.Add<T>(this.DangerousGetPinnableReference(), index);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly ref T GetPinnableReference()
		{
			if (this._length != 0)
			{
				return this.DangerousGetPinnableReference();
			}
			return Unsafe.AsRef<T>(null);
		}

		public void CopyTo([Nullable(new byte[] { 0, 1 })] global::System.Span<T> destination)
		{
			if (!this.TryCopyTo(destination))
			{
				ThrowHelper.ThrowArgumentException_DestinationTooShort();
			}
		}

		public bool TryCopyTo([Nullable(new byte[] { 0, 1 })] global::System.Span<T> destination)
		{
			int length = this._length;
			int length2 = destination.Length;
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

		public static bool operator ==([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlySpan<T> left, [Nullable(new byte[] { 0, 1 })] global::System.ReadOnlySpan<T> right)
		{
			return left._length == right._length && Unsafe.AreSame<T>(left.DangerousGetPinnableReference(), right.DangerousGetPinnableReference());
		}

		public unsafe override string ToString()
		{
			if (typeof(T) == typeof(char))
			{
				if (this._byteOffset == (IntPtr)RuntimeHelpers.OffsetToStringData)
				{
					string text = Unsafe.As<object>(this._pinnable) as string;
					if (text != null && this._length == text.Length)
					{
						return text;
					}
				}
				fixed (char* ptr = Unsafe.As<T, char>(this.DangerousGetPinnableReference()))
				{
					return new string(ptr, 0, this._length);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 2);
			defaultInterpolatedStringHandler.AppendLiteral("System.ReadOnlySpan<");
			defaultInterpolatedStringHandler.AppendFormatted(typeof(T).Name);
			defaultInterpolatedStringHandler.AppendLiteral(">[");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this._length);
			defaultInterpolatedStringHandler.AppendLiteral("]");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(new byte[] { 0, 1 })]
		public global::System.ReadOnlySpan<T> Slice(int start)
		{
			if (start > this._length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			IntPtr intPtr = this._byteOffset.Add<T>(start);
			int num = this._length - start;
			return new global::System.ReadOnlySpan<T>(this._pinnable, intPtr, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(new byte[] { 0, 1 })]
		public global::System.ReadOnlySpan<T> Slice(int start, int length)
		{
			if (start > this._length || length > this._length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			IntPtr intPtr = this._byteOffset.Add<T>(start);
			return new global::System.ReadOnlySpan<T>(this._pinnable, intPtr, length);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ref T DangerousGetPinnableReference()
		{
			return Unsafe.AddByteOffset<T>(ILHelpers.ObjectAsRef<T>(this._pinnable), this._byteOffset);
		}

		[Nullable(2)]
		internal object Pinnable
		{
			[NullableContext(2)]
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

		[Nullable(2)]
		private readonly object _pinnable;

		private readonly IntPtr _byteOffset;

		private readonly int _length;

		[Nullable(0)]
		public ref struct Enumerator
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Enumerator([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlySpan<T> span)
			{
				this._span = span;
				this._index = -1;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				int num = this._index + 1;
				if (num < this._span.Length)
				{
					this._index = num;
					return true;
				}
				return false;
			}

			public readonly ref T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this._span[this._index];
				}
			}

			[Nullable(new byte[] { 0, 1 })]
			private readonly global::System.ReadOnlySpan<T> _span;

			private int _index;
		}
	}
}
