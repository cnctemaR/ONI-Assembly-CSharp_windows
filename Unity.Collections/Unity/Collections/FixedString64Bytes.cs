using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
using UnityEngine;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct FixedString64Bytes : INativeList<byte>, IIndexable<byte>, IUTF8Bytes, IComparable<string>, IEquatable<string>, IComparable<FixedString32Bytes>, IEquatable<FixedString32Bytes>, IComparable<FixedString64Bytes>, IEquatable<FixedString64Bytes>, IComparable<FixedString128Bytes>, IEquatable<FixedString128Bytes>, IComparable<FixedString512Bytes>, IEquatable<FixedString512Bytes>, IComparable<FixedString4096Bytes>, IEquatable<FixedString4096Bytes>
	{
		public static int UTF8MaxLengthInBytes
		{
			get
			{
				return 61;
			}
		}

		[CreateProperty]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ExcludeFromBurstCompatTesting("Returns managed string")]
		public string Value
		{
			get
			{
				return this.ToString();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe readonly byte* GetUnsafePtr()
		{
			fixed (FixedBytes62* ptr = &this.bytes)
			{
				return (byte*)ptr;
			}
		}

		public unsafe int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return (int)this.utf8LengthInBytes;
			}
			set
			{
				this.utf8LengthInBytes = (ushort)value;
				this.GetUnsafePtr()[this.utf8LengthInBytes] = 0;
			}
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return 61;
			}
			set
			{
			}
		}

		public unsafe bool TryResize(int newLength, NativeArrayOptions clearOptions = NativeArrayOptions.ClearMemory)
		{
			if (newLength < 0 || newLength > 61)
			{
				return false;
			}
			if (newLength == (int)this.utf8LengthInBytes)
			{
				return true;
			}
			if (clearOptions == NativeArrayOptions.ClearMemory)
			{
				if (newLength > (int)this.utf8LengthInBytes)
				{
					UnsafeUtility.MemClear((void*)(this.GetUnsafePtr() + this.utf8LengthInBytes), (long)(newLength - (int)this.utf8LengthInBytes));
				}
				else
				{
					UnsafeUtility.MemClear((void*)(this.GetUnsafePtr() + newLength), (long)((int)this.utf8LengthInBytes - newLength));
				}
			}
			this.utf8LengthInBytes = (ushort)newLength;
			this.GetUnsafePtr()[this.utf8LengthInBytes] = 0;
			return true;
		}

		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.utf8LengthInBytes == 0;
			}
		}

		public unsafe byte this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this.GetUnsafePtr()[index];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.GetUnsafePtr()[index] = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe ref byte ElementAt(int index)
		{
			return ref this.GetUnsafePtr()[index];
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

		public FixedString64Bytes.Enumerator GetEnumerator()
		{
			return new FixedString64Bytes.Enumerator(this);
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public int CompareTo(string other)
		{
			return this.ToString().CompareTo(other);
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public unsafe bool Equals(string other)
		{
			int num = (int)this.utf8LengthInBytes;
			int length = other.Length;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in this.bytes);
			char* ptr2 = other;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			return UTF8ArrayUnsafeUtility.StrCmp(ptr, num, ptr2, length) == 0;
		}

		public ref FixedList64Bytes<byte> AsFixedList()
		{
			return UnsafeUtility.AsRef<FixedList64Bytes<byte>>(UnsafeUtility.AddressOf<FixedString64Bytes>(ref this));
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public FixedString64Bytes(string source)
		{
			this = default(FixedString64Bytes);
			this.Initialize(source);
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		internal CopyError Initialize(string source)
		{
			return (ref this).CopyFromTruncated<FixedString64Bytes>(source);
		}

		public FixedString64Bytes(Unicode.Rune rune, int count = 1)
		{
			this = default(FixedString64Bytes);
			this.Initialize(rune, count);
		}

		internal FormatError Initialize(Unicode.Rune rune, int count = 1)
		{
			this = default(FixedString64Bytes);
			return (ref this).Append<FixedString64Bytes>(rune, count);
		}

		internal unsafe FormatError Initialize(byte* srcBytes, int srcLength)
		{
			this.bytes = default(FixedBytes62);
			this.utf8LengthInBytes = 0;
			int num = 0;
			FormatError formatError = UTF8ArrayUnsafeUtility.AppendUTF8Bytes(this.GetUnsafePtr(), ref num, 61, srcBytes, srcLength);
			if (formatError != FormatError.None)
			{
				return formatError;
			}
			this.Length = num;
			return FormatError.None;
		}

		public FixedString64Bytes(NativeText.ReadOnly other)
		{
			this = default(FixedString64Bytes);
			this.Initialize(other.GetUnsafePtr(), other.Length);
		}

		public FixedString64Bytes(in UnsafeText other)
		{
			this = default(FixedString64Bytes);
			UnsafeText unsafeText = other;
			this.Initialize(unsafeText.GetUnsafePtr(), other.Length);
		}

		public int CompareTo(FixedString32Bytes other)
		{
			return (ref this).CompareTo<FixedString64Bytes, FixedString32Bytes>(in other);
		}

		public FixedString64Bytes(in FixedString32Bytes other)
		{
			this = default(FixedString64Bytes);
			this.Initialize(in other);
		}

		internal unsafe FormatError Initialize(in FixedString32Bytes other)
		{
			return this.Initialize((byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes30>(in other.bytes), (int)other.utf8LengthInBytes);
		}

		public unsafe static bool operator ==(in FixedString64Bytes a, in FixedString32Bytes b)
		{
			int num = (int)a.utf8LengthInBytes;
			int num2 = (int)b.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in a.bytes);
			byte* ptr2 = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes30>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(ptr, num, ptr2, num2);
		}

		public static bool operator !=(in FixedString64Bytes a, in FixedString32Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString32Bytes other)
		{
			return (in this) == (in other);
		}

		public int CompareTo(FixedString64Bytes other)
		{
			return (ref this).CompareTo<FixedString64Bytes, FixedString64Bytes>(in other);
		}

		public FixedString64Bytes(in FixedString64Bytes other)
		{
			this = default(FixedString64Bytes);
			this.Initialize(in other);
		}

		internal unsafe FormatError Initialize(in FixedString64Bytes other)
		{
			return this.Initialize((byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in other.bytes), (int)other.utf8LengthInBytes);
		}

		public unsafe static bool operator ==(in FixedString64Bytes a, in FixedString64Bytes b)
		{
			int num = (int)a.utf8LengthInBytes;
			int num2 = (int)b.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in a.bytes);
			byte* ptr2 = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(ptr, num, ptr2, num2);
		}

		public static bool operator !=(in FixedString64Bytes a, in FixedString64Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString64Bytes other)
		{
			return (in this) == (in other);
		}

		public int CompareTo(FixedString128Bytes other)
		{
			return (ref this).CompareTo<FixedString64Bytes, FixedString128Bytes>(in other);
		}

		public FixedString64Bytes(in FixedString128Bytes other)
		{
			this = default(FixedString64Bytes);
			this.Initialize(in other);
		}

		internal unsafe FormatError Initialize(in FixedString128Bytes other)
		{
			return this.Initialize((byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes126>(in other.bytes), (int)other.utf8LengthInBytes);
		}

		public unsafe static bool operator ==(in FixedString64Bytes a, in FixedString128Bytes b)
		{
			int num = (int)a.utf8LengthInBytes;
			int num2 = (int)b.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in a.bytes);
			byte* ptr2 = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes126>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(ptr, num, ptr2, num2);
		}

		public static bool operator !=(in FixedString64Bytes a, in FixedString128Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString128Bytes other)
		{
			return (in this) == (in other);
		}

		public static implicit operator FixedString128Bytes(in FixedString64Bytes fs)
		{
			return new FixedString128Bytes(in fs);
		}

		public int CompareTo(FixedString512Bytes other)
		{
			return (ref this).CompareTo<FixedString64Bytes, FixedString512Bytes>(in other);
		}

		public FixedString64Bytes(in FixedString512Bytes other)
		{
			this = default(FixedString64Bytes);
			this.Initialize(in other);
		}

		internal unsafe FormatError Initialize(in FixedString512Bytes other)
		{
			return this.Initialize((byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes510>(in other.bytes), (int)other.utf8LengthInBytes);
		}

		public unsafe static bool operator ==(in FixedString64Bytes a, in FixedString512Bytes b)
		{
			int num = (int)a.utf8LengthInBytes;
			int num2 = (int)b.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in a.bytes);
			byte* ptr2 = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes510>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(ptr, num, ptr2, num2);
		}

		public static bool operator !=(in FixedString64Bytes a, in FixedString512Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString512Bytes other)
		{
			return (in this) == (in other);
		}

		public static implicit operator FixedString512Bytes(in FixedString64Bytes fs)
		{
			return new FixedString512Bytes(in fs);
		}

		public int CompareTo(FixedString4096Bytes other)
		{
			return (ref this).CompareTo<FixedString64Bytes, FixedString4096Bytes>(in other);
		}

		public FixedString64Bytes(in FixedString4096Bytes other)
		{
			this = default(FixedString64Bytes);
			this.Initialize(in other);
		}

		internal unsafe FormatError Initialize(in FixedString4096Bytes other)
		{
			return this.Initialize((byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes4094>(in other.bytes), (int)other.utf8LengthInBytes);
		}

		public unsafe static bool operator ==(in FixedString64Bytes a, in FixedString4096Bytes b)
		{
			int num = (int)a.utf8LengthInBytes;
			int num2 = (int)b.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in a.bytes);
			byte* ptr2 = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes4094>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(ptr, num, ptr2, num2);
		}

		public static bool operator !=(in FixedString64Bytes a, in FixedString4096Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString4096Bytes other)
		{
			return (in this) == (in other);
		}

		public static implicit operator FixedString4096Bytes(in FixedString64Bytes fs)
		{
			return new FixedString4096Bytes(in fs);
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static implicit operator FixedString64Bytes(string b)
		{
			return new FixedString64Bytes(b);
		}

		[ExcludeFromBurstCompatTesting("Returns managed string")]
		public override string ToString()
		{
			return (ref this).ConvertToString<FixedString64Bytes>();
		}

		public override int GetHashCode()
		{
			return (ref this).ComputeHashCode<FixedString64Bytes>();
		}

		[ExcludeFromBurstCompatTesting("Takes managed object")]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			string text = obj as string;
			if (text != null)
			{
				return this.Equals(text);
			}
			if (obj is FixedString32Bytes)
			{
				FixedString32Bytes fixedString32Bytes = (FixedString32Bytes)obj;
				return this.Equals(fixedString32Bytes);
			}
			if (obj is FixedString64Bytes)
			{
				FixedString64Bytes fixedString64Bytes = (FixedString64Bytes)obj;
				return this.Equals(fixedString64Bytes);
			}
			if (obj is FixedString128Bytes)
			{
				FixedString128Bytes fixedString128Bytes = (FixedString128Bytes)obj;
				return this.Equals(fixedString128Bytes);
			}
			if (obj is FixedString512Bytes)
			{
				FixedString512Bytes fixedString512Bytes = (FixedString512Bytes)obj;
				return this.Equals(fixedString512Bytes);
			}
			if (obj is FixedString4096Bytes)
			{
				FixedString4096Bytes fixedString4096Bytes = (FixedString4096Bytes)obj;
				return this.Equals(fixedString4096Bytes);
			}
			return false;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly void CheckIndexInRange(int index)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} must be positive.", index));
			}
			if (index >= (int)this.utf8LengthInBytes)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} is out of range in FixedString64Bytes of '{1}' Length.", index, this.utf8LengthInBytes));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private void CheckLengthInRange(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Length {0} must be positive.", length));
			}
			if (length > 61)
			{
				throw new ArgumentOutOfRangeException(string.Format("Length {0} is out of range in FixedString64Bytes of '{1}' Capacity.", length, 61));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private void CheckCapacityInRange(int capacity)
		{
			if (capacity > 61)
			{
				throw new ArgumentOutOfRangeException(string.Format("Capacity {0} must be lower than {1}.", capacity, 61));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void CheckCopyError(CopyError error, string source)
		{
			if (error != CopyError.None)
			{
				throw new ArgumentException(string.Format("FixedString64Bytes: {0} while copying \"{1}\"", error, source));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		private static void CheckFormatError(FormatError error)
		{
			if (error != FormatError.None)
			{
				throw new ArgumentException("Source is too long to fit into fixed string of this size");
			}
		}

		internal const ushort utf8MaxLengthInBytes = 61;

		[SerializeField]
		internal ushort utf8LengthInBytes;

		[SerializeField]
		internal FixedBytes62 bytes;

		public struct Enumerator : IEnumerator
		{
			public Enumerator(FixedString64Bytes other)
			{
				this.target = other;
				this.offset = 0;
				this.current = default(Unicode.Rune);
			}

			public void Dispose()
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

			public Unicode.Rune Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private FixedString64Bytes target;

			private int offset;

			private Unicode.Rune current;
		}
	}
}
