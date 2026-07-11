using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[DebuggerTypeProxy(typeof(MemoryDebugView<>))]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly struct ReadOnlyMemory<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory(T[] array)
		{
			if (array == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			this._arrayOrOwnedMemory = array;
			this._index = 0;
			this._length = array.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory(T[] array, int start, int length)
		{
			if (array == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			if (start > array.Length || length > array.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			this._arrayOrOwnedMemory = array;
			this._index = start;
			this._length = length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ReadOnlyMemory(OwnedMemory<T> owner, int index, int length)
		{
			if (owner == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.ownedMemory);
			}
			if (index < 0 || length < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			this._arrayOrOwnedMemory = owner;
			this._index = index | int.MinValue;
			this._length = length;
		}

		private string DebuggerDisplay
		{
			get
			{
				return string.Format("{{{0}[{1}]}}", typeof(T).Name, this._length);
			}
		}

		public static implicit operator ReadOnlyMemory<T>(T[] array)
		{
			return new ReadOnlyMemory<T>(array);
		}

		public static implicit operator ReadOnlyMemory<T>(ArraySegment<T> arraySegment)
		{
			return new ReadOnlyMemory<T>(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}

		public static ReadOnlyMemory<T> Empty { get; } = SpanHelpers.PerTypeValues<T>.EmptyArray;

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory<T> Slice(int start)
		{
			if (start > this._length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			if (this._index < 0)
			{
				return new ReadOnlyMemory<T>((OwnedMemory<T>)this._arrayOrOwnedMemory, (this._index & int.MaxValue) + start, this._length - start);
			}
			return new ReadOnlyMemory<T>((T[])this._arrayOrOwnedMemory, this._index + start, this._length - start);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemory<T> Slice(int start, int length)
		{
			if (start > this._length || length > this._length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			if (this._index < 0)
			{
				return new ReadOnlyMemory<T>((OwnedMemory<T>)this._arrayOrOwnedMemory, (this._index & int.MaxValue) + start, length);
			}
			return new ReadOnlyMemory<T>((T[])this._arrayOrOwnedMemory, this._index + start, length);
		}

		public ReadOnlySpan<T> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (this._index < 0)
				{
					return ((OwnedMemory<T>)this._arrayOrOwnedMemory).Span.Slice(this._index & int.MaxValue, this._length);
				}
				return new ReadOnlySpan<T>((T[])this._arrayOrOwnedMemory, this._index, this._length);
			}
		}

		public unsafe MemoryHandle Retain(bool pin = false)
		{
			MemoryHandle memoryHandle;
			if (pin)
			{
				if (this._index < 0)
				{
					memoryHandle = ((OwnedMemory<T>)this._arrayOrOwnedMemory).Pin();
					memoryHandle.AddOffset((this._index & int.MaxValue) * Unsafe.SizeOf<T>());
				}
				else
				{
					GCHandle gchandle = GCHandle.Alloc((T[])this._arrayOrOwnedMemory, GCHandleType.Pinned);
					void* ptr = Unsafe.Add<T>((void*)gchandle.AddrOfPinnedObject(), this._index);
					memoryHandle = new MemoryHandle(null, ptr, gchandle);
				}
			}
			else if (this._index < 0)
			{
				((OwnedMemory<T>)this._arrayOrOwnedMemory).Retain();
				memoryHandle = new MemoryHandle((OwnedMemory<T>)this._arrayOrOwnedMemory, null, default(GCHandle));
			}
			else
			{
				memoryHandle = new MemoryHandle(null, null, default(GCHandle));
			}
			return memoryHandle;
		}

		public bool DangerousTryGetArray(out ArraySegment<T> arraySegment)
		{
			if (this._index >= 0)
			{
				arraySegment = new ArraySegment<T>((T[])this._arrayOrOwnedMemory, this._index, this._length);
				return true;
			}
			ArraySegment<T> arraySegment2;
			if (((OwnedMemory<T>)this._arrayOrOwnedMemory).TryGetArray(out arraySegment2))
			{
				arraySegment = new ArraySegment<T>(arraySegment2.Array, arraySegment2.Offset + (this._index & int.MaxValue), this._length);
				return true;
			}
			arraySegment = default(ArraySegment<T>);
			return false;
		}

		public T[] ToArray()
		{
			return this.Span.ToArray();
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is ReadOnlyMemory<T>;
			ReadOnlyMemory<T> readOnlyMemory = (flag ? ((ReadOnlyMemory<T>)obj) : default(ReadOnlyMemory<T>));
			if (flag)
			{
				return this.Equals(readOnlyMemory);
			}
			bool flag2 = obj is Memory<T>;
			Memory<T> memory = (flag2 ? ((Memory<T>)obj) : default(Memory<T>));
			return flag2 && this.Equals(memory);
		}

		public bool Equals(ReadOnlyMemory<T> other)
		{
			return this._arrayOrOwnedMemory == other._arrayOrOwnedMemory && this._index == other._index && this._length == other._length;
		}

		public override int GetHashCode()
		{
			return ReadOnlyMemory<T>.CombineHashCodes(this._arrayOrOwnedMemory.GetHashCode(), (this._index & int.MaxValue).GetHashCode(), this._length.GetHashCode());
		}

		private static int CombineHashCodes(int left, int right)
		{
			return ((left << 5) + left) ^ right;
		}

		private static int CombineHashCodes(int h1, int h2, int h3)
		{
			return ReadOnlyMemory<T>.CombineHashCodes(ReadOnlyMemory<T>.CombineHashCodes(h1, h2), h3);
		}

		private readonly object _arrayOrOwnedMemory;

		private readonly int _index;

		private readonly int _length;

		private const int RemoveOwnedFlagBitMask = 2147483647;
	}
}
