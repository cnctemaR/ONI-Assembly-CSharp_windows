using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[BurstCompatible]
	[Obsolete("HeapString has been removed and replaced with NativeText (RemovedAfter 2021-07-21) (UnityUpgradable) -> NativeText", false)]
	public struct HeapString : INativeList<byte>, IIndexable<byte>, IDisposable, IUTF8Bytes, IComparable<string>, IEquatable<string>, IComparable<HeapString>, IEquatable<HeapString>, IComparable<FixedString32Bytes>, IEquatable<FixedString32Bytes>, IComparable<FixedString64Bytes>, IEquatable<FixedString64Bytes>, IComparable<FixedString128Bytes>, IEquatable<FixedString128Bytes>, IComparable<FixedString512Bytes>, IEquatable<FixedString512Bytes>, IComparable<FixedString4096Bytes>, IEquatable<FixedString4096Bytes>
	{
		public int Length
		{
			get
			{
				return this.m_Data.Length - 1;
			}
			set
			{
				this.m_Data.Resize(value + 1, NativeArrayOptions.UninitializedMemory);
				this.m_Data[value] = 0;
			}
		}

		public int Capacity
		{
			get
			{
				return this.m_Data.Capacity - 1;
			}
			set
			{
				this.m_Data.Capacity = value + 1;
			}
		}

		public bool TryResize(int newLength, NativeArrayOptions clearOptions = NativeArrayOptions.ClearMemory)
		{
			this.Length = newLength;
			return true;
		}

		public bool IsEmpty
		{
			get
			{
				return this.m_Data.Length == 1;
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.m_Data.IsCreated;
			}
		}

		public unsafe byte* GetUnsafePtr()
		{
			return (byte*)this.m_Data.GetUnsafePtr<byte>();
		}

		public byte this[int index]
		{
			get
			{
				return this.m_Data[index];
			}
			set
			{
				this.m_Data[index] = value;
			}
		}

		public ref byte ElementAt(int index)
		{
			return this.m_Data.ElementAt(index);
		}

		public void Clear()
		{
			this.Length = 0;
		}

		public void Add(in byte value)
		{
			int length = this.Length;
			this.Length = length + 1;
			this[length] = value;
		}

		public int CompareTo(HeapString other)
		{
			return (ref this).CompareTo<HeapString, HeapString>(in other);
		}

		public bool Equals(HeapString other)
		{
			return (ref this).Equals<HeapString, HeapString>(in other);
		}

		public void Dispose()
		{
			this.m_Data.Dispose();
		}

		[CreateProperty]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[NotBurstCompatible]
		public string Value
		{
			get
			{
				return this.ToString();
			}
		}

		public HeapString.Enumerator GetEnumerator()
		{
			return new HeapString.Enumerator(this);
		}

		[NotBurstCompatible]
		public int CompareTo(string other)
		{
			return this.ToString().CompareTo(other);
		}

		[NotBurstCompatible]
		public bool Equals(string other)
		{
			return this.ToString().Equals(other);
		}

		[NotBurstCompatible]
		public unsafe HeapString(string source, Allocator allocator)
		{
			this.m_Data = new NativeList<byte>(source.Length * 2 + 1, allocator);
			this.Length = source.Length * 2;
			fixed (string text = source)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				int num;
				if (UTF8ArrayUnsafeUtility.Copy(this.GetUnsafePtr(), out num, this.Capacity, ptr, source.Length) != CopyError.None)
				{
					this.m_Data.Dispose();
					this.m_Data = default(NativeList<byte>);
				}
				this.Length = num;
			}
		}

		public HeapString(int capacity, Allocator allocator)
		{
			this.m_Data = new NativeList<byte>(capacity + 1, allocator);
			this.Length = 0;
		}

		public HeapString(Allocator allocator)
		{
			this.m_Data = new NativeList<byte>(129, allocator);
			this.Length = 0;
		}

		public int CompareTo(FixedString32Bytes other)
		{
			return (ref this).CompareTo<HeapString, FixedString32Bytes>(in other);
		}

		public unsafe HeapString(in FixedString32Bytes source, Allocator allocator)
		{
			this.m_Data = new NativeList<byte>((int)(source.utf8LengthInBytes + 1), allocator);
			this.Length = (int)source.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes30>(in source.bytes);
			byte* unsafePtr = (byte*)this.m_Data.GetUnsafePtr<byte>();
			UnsafeUtility.MemCpy((void*)unsafePtr, (void*)ptr, (long)((ulong)source.utf8LengthInBytes));
		}

		public unsafe static bool operator ==(in HeapString a, in FixedString32Bytes b)
		{
			HeapString heapString = *UnsafeUtilityExtensions.AsRef<HeapString>(in a);
			int length = heapString.Length;
			int utf8LengthInBytes = (int)b.utf8LengthInBytes;
			byte* unsafePtr = heapString.GetUnsafePtr();
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes30>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
		}

		public static bool operator !=(in HeapString a, in FixedString32Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString32Bytes other)
		{
			return (in this) == (in other);
		}

		public int CompareTo(FixedString64Bytes other)
		{
			return (ref this).CompareTo<HeapString, FixedString64Bytes>(in other);
		}

		public unsafe HeapString(in FixedString64Bytes source, Allocator allocator)
		{
			this.m_Data = new NativeList<byte>((int)(source.utf8LengthInBytes + 1), allocator);
			this.Length = (int)source.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in source.bytes);
			byte* unsafePtr = (byte*)this.m_Data.GetUnsafePtr<byte>();
			UnsafeUtility.MemCpy((void*)unsafePtr, (void*)ptr, (long)((ulong)source.utf8LengthInBytes));
		}

		public unsafe static bool operator ==(in HeapString a, in FixedString64Bytes b)
		{
			HeapString heapString = *UnsafeUtilityExtensions.AsRef<HeapString>(in a);
			int length = heapString.Length;
			int utf8LengthInBytes = (int)b.utf8LengthInBytes;
			byte* unsafePtr = heapString.GetUnsafePtr();
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
		}

		public static bool operator !=(in HeapString a, in FixedString64Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString64Bytes other)
		{
			return (in this) == (in other);
		}

		public int CompareTo(FixedString128Bytes other)
		{
			return (ref this).CompareTo<HeapString, FixedString128Bytes>(in other);
		}

		public unsafe HeapString(in FixedString128Bytes source, Allocator allocator)
		{
			this.m_Data = new NativeList<byte>((int)(source.utf8LengthInBytes + 1), allocator);
			this.Length = (int)source.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes126>(in source.bytes);
			byte* unsafePtr = (byte*)this.m_Data.GetUnsafePtr<byte>();
			UnsafeUtility.MemCpy((void*)unsafePtr, (void*)ptr, (long)((ulong)source.utf8LengthInBytes));
		}

		public unsafe static bool operator ==(in HeapString a, in FixedString128Bytes b)
		{
			HeapString heapString = *UnsafeUtilityExtensions.AsRef<HeapString>(in a);
			int length = heapString.Length;
			int utf8LengthInBytes = (int)b.utf8LengthInBytes;
			byte* unsafePtr = heapString.GetUnsafePtr();
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes126>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
		}

		public static bool operator !=(in HeapString a, in FixedString128Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString128Bytes other)
		{
			return (in this) == (in other);
		}

		public int CompareTo(FixedString512Bytes other)
		{
			return (ref this).CompareTo<HeapString, FixedString512Bytes>(in other);
		}

		public unsafe HeapString(in FixedString512Bytes source, Allocator allocator)
		{
			this.m_Data = new NativeList<byte>((int)(source.utf8LengthInBytes + 1), allocator);
			this.Length = (int)source.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes510>(in source.bytes);
			byte* unsafePtr = (byte*)this.m_Data.GetUnsafePtr<byte>();
			UnsafeUtility.MemCpy((void*)unsafePtr, (void*)ptr, (long)((ulong)source.utf8LengthInBytes));
		}

		public unsafe static bool operator ==(in HeapString a, in FixedString512Bytes b)
		{
			HeapString heapString = *UnsafeUtilityExtensions.AsRef<HeapString>(in a);
			int length = heapString.Length;
			int utf8LengthInBytes = (int)b.utf8LengthInBytes;
			byte* unsafePtr = heapString.GetUnsafePtr();
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes510>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
		}

		public static bool operator !=(in HeapString a, in FixedString512Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString512Bytes other)
		{
			return (in this) == (in other);
		}

		public int CompareTo(FixedString4096Bytes other)
		{
			return (ref this).CompareTo<HeapString, FixedString4096Bytes>(in other);
		}

		public unsafe HeapString(in FixedString4096Bytes source, Allocator allocator)
		{
			this.m_Data = new NativeList<byte>((int)(source.utf8LengthInBytes + 1), allocator);
			this.Length = (int)source.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes4094>(in source.bytes);
			byte* unsafePtr = (byte*)this.m_Data.GetUnsafePtr<byte>();
			UnsafeUtility.MemCpy((void*)unsafePtr, (void*)ptr, (long)((ulong)source.utf8LengthInBytes));
		}

		public unsafe static bool operator ==(in HeapString a, in FixedString4096Bytes b)
		{
			HeapString heapString = *UnsafeUtilityExtensions.AsRef<HeapString>(in a);
			int length = heapString.Length;
			int utf8LengthInBytes = (int)b.utf8LengthInBytes;
			byte* unsafePtr = heapString.GetUnsafePtr();
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes4094>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
		}

		public static bool operator !=(in HeapString a, in FixedString4096Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString4096Bytes other)
		{
			return (in this) == (in other);
		}

		[NotBurstCompatible]
		public override string ToString()
		{
			if (!this.m_Data.IsCreated)
			{
				return "";
			}
			return (ref this).ConvertToString<HeapString>();
		}

		public override int GetHashCode()
		{
			return (ref this).ComputeHashCode<HeapString>();
		}

		[NotBurstCompatible]
		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			string text = other as string;
			if (text != null)
			{
				return this.Equals(text);
			}
			if (other is HeapString)
			{
				HeapString heapString = (HeapString)other;
				return this.Equals(heapString);
			}
			if (other is FixedString32Bytes)
			{
				FixedString32Bytes fixedString32Bytes = (FixedString32Bytes)other;
				return this.Equals(fixedString32Bytes);
			}
			if (other is FixedString64Bytes)
			{
				FixedString64Bytes fixedString64Bytes = (FixedString64Bytes)other;
				return this.Equals(fixedString64Bytes);
			}
			if (other is FixedString128Bytes)
			{
				FixedString128Bytes fixedString128Bytes = (FixedString128Bytes)other;
				return this.Equals(fixedString128Bytes);
			}
			if (other is FixedString512Bytes)
			{
				FixedString512Bytes fixedString512Bytes = (FixedString512Bytes)other;
				return this.Equals(fixedString512Bytes);
			}
			if (other is FixedString4096Bytes)
			{
				FixedString4096Bytes fixedString4096Bytes = (FixedString4096Bytes)other;
				return this.Equals(fixedString4096Bytes);
			}
			return false;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckIndexInRange(int index)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} must be positive.", index));
			}
			if (index >= this.Length)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} is out of range in HeapString of {1} length.", index, this.Length));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void ThrowCopyError(CopyError error, string source)
		{
			throw new ArgumentException(string.Format("HeapString: {0} while copying \"{1}\"", error, source));
		}

		private NativeList<byte> m_Data;

		public struct Enumerator : IEnumerator<Unicode.Rune>, IEnumerator, IDisposable
		{
			public Enumerator(HeapString source)
			{
				this.target = source;
				this.offset = 0;
				this.current = default(Unicode.Rune);
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (this.offset >= this.target.Length)
				{
					return false;
				}
				Unicode.Utf8ToUcs(out this.current, this.target.GetUnsafePtr(), ref this.offset, this.target.Length);
				return true;
			}

			public void Reset()
			{
				this.offset = 0;
				this.current = default(Unicode.Rune);
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public Unicode.Rune Current
			{
				get
				{
					return this.current;
				}
			}

			private HeapString target;

			private int offset;

			private Unicode.Rune current;
		}
	}
}
