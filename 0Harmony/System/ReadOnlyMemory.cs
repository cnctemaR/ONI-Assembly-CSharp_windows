using System;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[NullableContext(2)]
	[Nullable(0)]
	[DebuggerTypeProxy(typeof(MemoryDebugView<>))]
	[DebuggerDisplay("{ToString(),raw}")]
	internal readonly struct ReadOnlyMemory<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory([Nullable(new byte[] { 2, 1 })] T[] array)
		{
			if (array == null)
			{
				this = default(global::System.ReadOnlyMemory<T>);
				return;
			}
			this._object = array;
			this._index = 0;
			this._length = array.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory([Nullable(new byte[] { 2, 1 })] T[] array, int start, int length)
		{
			if (array == null)
			{
				if (start != 0 || length != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException();
				}
				this = default(global::System.ReadOnlyMemory<T>);
				return;
			}
			if (start > array.Length || length > array.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			this._object = array;
			this._index = start;
			this._length = length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ReadOnlyMemory(object obj, int start, int length)
		{
			this._object = obj;
			this._index = start;
			this._length = length;
		}

		[return: Nullable(new byte[] { 0, 1 })]
		public static implicit operator global::System.ReadOnlyMemory<T>([Nullable(new byte[] { 2, 1 })] T[] array)
		{
			return new global::System.ReadOnlyMemory<T>(array);
		}

		[return: Nullable(new byte[] { 0, 1 })]
		public static implicit operator global::System.ReadOnlyMemory<T>([Nullable(new byte[] { 0, 1 })] ArraySegment<T> segment)
		{
			return new global::System.ReadOnlyMemory<T>(segment.Array, segment.Offset, segment.Count);
		}

		[Nullable(new byte[] { 0, 1 })]
		public static global::System.ReadOnlyMemory<T> Empty
		{
			[return: Nullable(new byte[] { 0, 1 })]
			get
			{
				return default(global::System.ReadOnlyMemory<T>);
			}
		}

		public int Length
		{
			get
			{
				return this._length & int.MaxValue;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return (this._length & int.MaxValue) == 0;
			}
		}

		[NullableContext(1)]
		public override string ToString()
		{
			if (!(typeof(T) == typeof(char)))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
				defaultInterpolatedStringHandler.AppendLiteral("System.ReadOnlyMemory<");
				defaultInterpolatedStringHandler.AppendFormatted(typeof(T).Name);
				defaultInterpolatedStringHandler.AppendLiteral(">[");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this._length & int.MaxValue);
				defaultInterpolatedStringHandler.AppendLiteral("]");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			string text = this._object as string;
			if (text == null)
			{
				return this.Span.ToString();
			}
			return text.Substring(this._index, this._length & int.MaxValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(new byte[] { 0, 1 })]
		public global::System.ReadOnlyMemory<T> Slice(int start)
		{
			int length = this._length;
			int num = length & int.MaxValue;
			if (start > num)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return new global::System.ReadOnlyMemory<T>(this._object, this._index + start, length - start);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(new byte[] { 0, 1 })]
		public global::System.ReadOnlyMemory<T> Slice(int start, int length)
		{
			int length2 = this._length;
			int num = this._length & int.MaxValue;
			if (start > num || length > num - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return new global::System.ReadOnlyMemory<T>(this._object, this._index + start, length | (length2 & int.MinValue));
		}

		[Nullable(new byte[] { 0, 1 })]
		public global::System.ReadOnlySpan<T> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[return: Nullable(new byte[] { 0, 1 })]
			get
			{
				if (this._index < 0)
				{
					return ((global::System.Buffers.MemoryManager<T>)this._object).GetSpan().Slice(this._index & int.MaxValue, this._length);
				}
				if (typeof(T) == typeof(char))
				{
					string text = this._object as string;
					if (text != null)
					{
						return new global::System.ReadOnlySpan<T>(text, (IntPtr)RuntimeHelpers.OffsetToStringData, text.Length).Slice(this._index, this._length);
					}
				}
				if (this._object != null)
				{
					return new global::System.ReadOnlySpan<T>((T[])this._object, this._index, this._length & int.MaxValue);
				}
				return default(global::System.ReadOnlySpan<T>);
			}
		}

		public void CopyTo([Nullable(new byte[] { 0, 1 })] global::System.Memory<T> destination)
		{
			this.Span.CopyTo(destination.Span);
		}

		public bool TryCopyTo([Nullable(new byte[] { 0, 1 })] global::System.Memory<T> destination)
		{
			return this.Span.TryCopyTo(destination.Span);
		}

		public unsafe global::System.Buffers.MemoryHandle Pin()
		{
			if (this._index < 0)
			{
				return ((global::System.Buffers.MemoryManager<T>)this._object).Pin(this._index & int.MaxValue);
			}
			if (typeof(T) == typeof(char))
			{
				string text = this._object as string;
				if (text != null)
				{
					GCHandle gchandle = GCHandle.Alloc(text, GCHandleType.Pinned);
					return new global::System.Buffers.MemoryHandle(Unsafe.Add<T>((void*)gchandle.AddrOfPinnedObject(), this._index), gchandle, null);
				}
			}
			T[] array = this._object as T[];
			if (array == null)
			{
				return default(global::System.Buffers.MemoryHandle);
			}
			if (this._length < 0)
			{
				return new global::System.Buffers.MemoryHandle(Unsafe.Add<T>(Unsafe.AsPointer<T>(global::System.Runtime.InteropServices.MemoryMarshal.GetReference<T>(array)), this._index), default(GCHandle), null);
			}
			GCHandle gchandle2 = GCHandle.Alloc(array, GCHandleType.Pinned);
			return new global::System.Buffers.MemoryHandle(Unsafe.Add<T>((void*)gchandle2.AddrOfPinnedObject(), this._index), gchandle2, null);
		}

		[NullableContext(1)]
		public T[] ToArray()
		{
			return this.Span.ToArray();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			if (obj is global::System.ReadOnlyMemory<T>)
			{
				global::System.ReadOnlyMemory<T> readOnlyMemory = (global::System.ReadOnlyMemory<T>)obj;
				return this.Equals(readOnlyMemory);
			}
			if (obj is global::System.Memory<T>)
			{
				global::System.Memory<T> memory = (global::System.Memory<T>)obj;
				return this.Equals(memory);
			}
			return false;
		}

		public bool Equals([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlyMemory<T> other)
		{
			return this._object == other._object && this._index == other._index && this._length == other._length;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			if (this._object == null)
			{
				return 0;
			}
			return global::System.HashCode.Combine<object, int, int>(this._object, this._index, this._length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal object GetObjectStartLength(out int start, out int length)
		{
			start = this._index;
			length = this._length;
			return this._object;
		}

		private readonly object _object;

		private readonly int _index;

		private readonly int _length;

		internal const int RemoveFlagsBitMask = 2147483647;
	}
}
