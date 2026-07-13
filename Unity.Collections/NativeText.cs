using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[DebuggerDisplay("Length = {Length}")]
	[BurstCompatible]
	public struct NativeText : INativeList<byte>, IIndexable<byte>, INativeDisposable, IDisposable, IUTF8Bytes, IComparable<string>, IEquatable<string>, IComparable<NativeText>, IEquatable<NativeText>, IComparable<FixedString32Bytes>, IEquatable<FixedString32Bytes>, IComparable<FixedString64Bytes>, IEquatable<FixedString64Bytes>, IComparable<FixedString128Bytes>, IEquatable<FixedString128Bytes>, IComparable<FixedString512Bytes>, IEquatable<FixedString512Bytes>, IComparable<FixedString4096Bytes>, IEquatable<FixedString4096Bytes>
	{
		[NotBurstCompatible]
		public NativeText(string source, Allocator allocator)
		{
			this = new NativeText(source, allocator);
		}

		[NotBurstCompatible]
		public unsafe NativeText(string source, AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeText(source.Length * 2, allocator);
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
					this.m_Data->Dispose();
					void* ptr2 = (ref allocator).Allocate<AllocatorManager.AllocatorHandle>(sizeof(UnsafeText), 16, 1);
					this.m_Data = (UnsafeText*)ptr2;
					*this.m_Data = default(UnsafeText);
				}
				this.Length = num;
			}
		}

		private unsafe NativeText(int capacity, AllocatorManager.AllocatorHandle allocator, int disposeSentinelStackDepth)
		{
			this = default(NativeText);
			void* ptr = (ref allocator).Allocate<AllocatorManager.AllocatorHandle>(sizeof(UnsafeText), 16, 1);
			this.m_Data = (UnsafeText*)ptr;
			*this.m_Data = new UnsafeText(capacity, allocator);
		}

		public NativeText(int capacity, Allocator allocator)
		{
			this = new NativeText(capacity, allocator);
		}

		public NativeText(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeText(capacity, allocator, 2);
		}

		public NativeText(Allocator allocator)
		{
			this = new NativeText(allocator);
		}

		public NativeText(AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeText(512, allocator);
		}

		public unsafe NativeText(in FixedString32Bytes source, AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeText((int)source.utf8LengthInBytes, allocator);
			this.Length = (int)source.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes30>(in source.bytes);
			UnsafeUtility.MemCpy((void*)this.m_Data->GetUnsafePtr(), (void*)ptr, (long)((ulong)source.utf8LengthInBytes));
		}

		public NativeText(in FixedString32Bytes source, Allocator allocator)
		{
			this = new NativeText(in source, allocator);
		}

		public unsafe NativeText(in FixedString64Bytes source, AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeText((int)source.utf8LengthInBytes, allocator);
			this.Length = (int)source.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in source.bytes);
			UnsafeUtility.MemCpy((void*)this.m_Data->GetUnsafePtr(), (void*)ptr, (long)((ulong)source.utf8LengthInBytes));
		}

		public NativeText(in FixedString64Bytes source, Allocator allocator)
		{
			this = new NativeText(in source, allocator);
		}

		public unsafe NativeText(in FixedString128Bytes source, AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeText((int)source.utf8LengthInBytes, allocator);
			this.Length = (int)source.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes126>(in source.bytes);
			UnsafeUtility.MemCpy((void*)this.m_Data->GetUnsafePtr(), (void*)ptr, (long)((ulong)source.utf8LengthInBytes));
		}

		public NativeText(in FixedString128Bytes source, Allocator allocator)
		{
			this = new NativeText(in source, allocator);
		}

		public unsafe NativeText(in FixedString512Bytes source, AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeText((int)source.utf8LengthInBytes, allocator);
			this.Length = (int)source.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes510>(in source.bytes);
			UnsafeUtility.MemCpy((void*)this.m_Data->GetUnsafePtr(), (void*)ptr, (long)((ulong)source.utf8LengthInBytes));
		}

		public NativeText(in FixedString512Bytes source, Allocator allocator)
		{
			this = new NativeText(in source, allocator);
		}

		public unsafe NativeText(in FixedString4096Bytes source, AllocatorManager.AllocatorHandle allocator)
		{
			this = new NativeText((int)source.utf8LengthInBytes, allocator);
			this.Length = (int)source.utf8LengthInBytes;
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes4094>(in source.bytes);
			UnsafeUtility.MemCpy((void*)this.m_Data->GetUnsafePtr(), (void*)ptr, (long)((ulong)source.utf8LengthInBytes));
		}

		public NativeText(in FixedString4096Bytes source, Allocator allocator)
		{
			this = new NativeText(in source, allocator);
		}

		public unsafe int Length
		{
			get
			{
				return this.m_Data->Length;
			}
			set
			{
				this.m_Data->Length = value;
			}
		}

		public unsafe int Capacity
		{
			get
			{
				return this.m_Data->Capacity;
			}
			set
			{
				this.m_Data->Capacity = value;
			}
		}

		public bool TryResize(int newLength, NativeArrayOptions clearOptions = NativeArrayOptions.ClearMemory)
		{
			this.Length = newLength;
			return true;
		}

		public unsafe bool IsEmpty
		{
			get
			{
				return !this.IsCreated || this.m_Data->IsEmpty;
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.m_Data != null;
			}
		}

		public unsafe byte* GetUnsafePtr()
		{
			return this.m_Data->GetUnsafePtr();
		}

		public unsafe byte this[int index]
		{
			get
			{
				return *this.m_Data->ElementAt(index);
			}
			set
			{
				*this.m_Data->ElementAt(index) = value;
			}
		}

		public unsafe ref byte ElementAt(int index)
		{
			return this.m_Data->ElementAt(index);
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

		public unsafe int CompareTo(NativeText other)
		{
			return (ref this).CompareTo<NativeText, UnsafeText>(in *other.m_Data);
		}

		public unsafe bool Equals(NativeText other)
		{
			return (ref this).Equals<NativeText, UnsafeText>(in *other.m_Data);
		}

		public int CompareTo(NativeText.ReadOnly other)
		{
			return (ref this).CompareTo<NativeText, NativeText.ReadOnly>(in other);
		}

		public unsafe bool Equals(NativeText.ReadOnly other)
		{
			return (ref this).Equals<NativeText, UnsafeText>(in *other.m_Data);
		}

		public unsafe void Dispose()
		{
			AllocatorManager.AllocatorHandle allocator = this.m_Data->m_UntypedListData.Allocator;
			this.m_Data->Dispose();
			AllocatorManager.Free<UnsafeText>(allocator, this.m_Data, 1);
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			return this.m_Data->Dispose(inputDeps);
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

		public NativeText.Enumerator GetEnumerator()
		{
			return new NativeText.Enumerator(this);
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

		public int CompareTo(FixedString32Bytes other)
		{
			return (ref this).CompareTo<NativeText, FixedString32Bytes>(in other);
		}

		public unsafe static bool operator ==(in NativeText a, in FixedString32Bytes b)
		{
			NativeText nativeText = *UnsafeUtilityExtensions.AsRef<NativeText>(in a);
			int length = nativeText.Length;
			int utf8LengthInBytes = (int)b.utf8LengthInBytes;
			byte* unsafePtr = nativeText.GetUnsafePtr();
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes30>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
		}

		public static bool operator !=(in NativeText a, in FixedString32Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString32Bytes other)
		{
			return (in this) == (in other);
		}

		public int CompareTo(FixedString64Bytes other)
		{
			return (ref this).CompareTo<NativeText, FixedString64Bytes>(in other);
		}

		public unsafe static bool operator ==(in NativeText a, in FixedString64Bytes b)
		{
			NativeText nativeText = *UnsafeUtilityExtensions.AsRef<NativeText>(in a);
			int length = nativeText.Length;
			int utf8LengthInBytes = (int)b.utf8LengthInBytes;
			byte* unsafePtr = nativeText.GetUnsafePtr();
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
		}

		public static bool operator !=(in NativeText a, in FixedString64Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString64Bytes other)
		{
			return (in this) == (in other);
		}

		public int CompareTo(FixedString128Bytes other)
		{
			return (ref this).CompareTo<NativeText, FixedString128Bytes>(in other);
		}

		public unsafe static bool operator ==(in NativeText a, in FixedString128Bytes b)
		{
			NativeText nativeText = *UnsafeUtilityExtensions.AsRef<NativeText>(in a);
			int length = nativeText.Length;
			int utf8LengthInBytes = (int)b.utf8LengthInBytes;
			byte* unsafePtr = nativeText.GetUnsafePtr();
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes126>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
		}

		public static bool operator !=(in NativeText a, in FixedString128Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString128Bytes other)
		{
			return (in this) == (in other);
		}

		public int CompareTo(FixedString512Bytes other)
		{
			return (ref this).CompareTo<NativeText, FixedString512Bytes>(in other);
		}

		public unsafe static bool operator ==(in NativeText a, in FixedString512Bytes b)
		{
			NativeText nativeText = *UnsafeUtilityExtensions.AsRef<NativeText>(in a);
			int length = nativeText.Length;
			int utf8LengthInBytes = (int)b.utf8LengthInBytes;
			byte* unsafePtr = nativeText.GetUnsafePtr();
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes510>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
		}

		public static bool operator !=(in NativeText a, in FixedString512Bytes b)
		{
			return !((in a) == (in b));
		}

		public bool Equals(FixedString512Bytes other)
		{
			return (in this) == (in other);
		}

		public int CompareTo(FixedString4096Bytes other)
		{
			return (ref this).CompareTo<NativeText, FixedString4096Bytes>(in other);
		}

		public unsafe static bool operator ==(in NativeText a, in FixedString4096Bytes b)
		{
			NativeText nativeText = *UnsafeUtilityExtensions.AsRef<NativeText>(in a);
			int length = nativeText.Length;
			int utf8LengthInBytes = (int)b.utf8LengthInBytes;
			byte* unsafePtr = nativeText.GetUnsafePtr();
			byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes4094>(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
		}

		public static bool operator !=(in NativeText a, in FixedString4096Bytes b)
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
			if (this.m_Data == null)
			{
				return "";
			}
			return (ref this).ConvertToString<NativeText>();
		}

		public override int GetHashCode()
		{
			return (ref this).ComputeHashCode<NativeText>();
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
			if (other is NativeText)
			{
				NativeText nativeText = (NativeText)other;
				return this.Equals(nativeText);
			}
			if (other is NativeText.ReadOnly)
			{
				NativeText.ReadOnly readOnly = (NativeText.ReadOnly)other;
				return this.Equals(readOnly);
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
		internal unsafe static void CheckNull(void* dataPtr)
		{
			if (dataPtr == null)
			{
				throw new Exception("NativeText has yet to be created or has been destroyed!");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckWrite()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckWriteAndBumpSecondaryVersion()
		{
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
				throw new IndexOutOfRangeException(string.Format("Index {0} is out of range in NativeText of {1} length.", index, this.Length));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void ThrowCopyError(CopyError error, string source)
		{
			throw new ArgumentException(string.Format("NativeText: {0} while copying \"{1}\"", error, source));
		}

		public NativeText.ReadOnly AsReadOnly()
		{
			return new NativeText.ReadOnly(this.m_Data);
		}

		[NativeDisableUnsafePtrRestriction]
		private unsafe UnsafeText* m_Data;

		public struct Enumerator : IEnumerator<Unicode.Rune>, IEnumerator, IDisposable
		{
			public Enumerator(NativeText source)
			{
				this.target = source.AsReadOnly();
				this.offset = 0;
				this.current = default(Unicode.Rune);
			}

			public Enumerator(NativeText.ReadOnly source)
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

			private NativeText.ReadOnly target;

			private int offset;

			private Unicode.Rune current;
		}

		[NativeContainer]
		[NativeContainerIsReadOnly]
		public struct ReadOnly : INativeList<byte>, IIndexable<byte>, IUTF8Bytes, IComparable<string>, IEquatable<string>, IComparable<NativeText>, IEquatable<NativeText>, IComparable<FixedString32Bytes>, IEquatable<FixedString32Bytes>, IComparable<FixedString64Bytes>, IEquatable<FixedString64Bytes>, IComparable<FixedString128Bytes>, IEquatable<FixedString128Bytes>, IComparable<FixedString512Bytes>, IEquatable<FixedString512Bytes>, IComparable<FixedString4096Bytes>, IEquatable<FixedString4096Bytes>
		{
			internal unsafe ReadOnly(UnsafeText* text)
			{
				this.m_Data = text;
			}

			public unsafe int Capacity
			{
				get
				{
					return this.m_Data->Capacity;
				}
				set
				{
				}
			}

			public unsafe bool IsEmpty
			{
				get
				{
					return this.m_Data == null || this.m_Data->IsEmpty;
				}
				set
				{
				}
			}

			public unsafe int Length
			{
				get
				{
					return this.m_Data->Length;
				}
				set
				{
				}
			}

			public unsafe byte this[int index]
			{
				get
				{
					return *this.m_Data->ElementAt(index);
				}
				set
				{
				}
			}

			public void Clear()
			{
			}

			public ref byte ElementAt(int index)
			{
				throw new NotSupportedException("Trying to retrieve non-readonly ref to NativeText.ReadOnly data. This is not permitted.");
			}

			public unsafe byte* GetUnsafePtr()
			{
				return this.m_Data->GetUnsafePtr();
			}

			public bool TryResize(int newLength, NativeArrayOptions clearOptions = NativeArrayOptions.ClearMemory)
			{
				return false;
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			internal unsafe static void CheckNull(void* dataPtr)
			{
				if (dataPtr == null)
				{
					throw new Exception("NativeText.ReadOnly has yet to be created or has been destroyed!");
				}
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private void CheckRead()
			{
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private void ErrorWrite()
			{
			}

			[NotBurstCompatible]
			public unsafe int CompareTo(string other)
			{
				return this.m_Data->ToString().CompareTo(other);
			}

			[NotBurstCompatible]
			public unsafe bool Equals(string other)
			{
				return this.m_Data->ToString().Equals(other);
			}

			public unsafe int CompareTo(NativeText.ReadOnly other)
			{
				return (ref *this.m_Data).CompareTo<UnsafeText, UnsafeText>(in *other.m_Data);
			}

			public unsafe bool Equals(NativeText.ReadOnly other)
			{
				return (ref *this.m_Data).Equals<UnsafeText, UnsafeText>(in *other.m_Data);
			}

			public unsafe int CompareTo(NativeText other)
			{
				return (ref this).CompareTo<NativeText.ReadOnly, UnsafeText>(in *other.m_Data);
			}

			public unsafe bool Equals(NativeText other)
			{
				return (ref this).Equals<NativeText.ReadOnly, UnsafeText>(in *other.m_Data);
			}

			public int CompareTo(FixedString32Bytes other)
			{
				return (ref this).CompareTo<NativeText.ReadOnly, FixedString32Bytes>(in other);
			}

			public unsafe static bool operator ==(in NativeText.ReadOnly a, in FixedString32Bytes b)
			{
				UnsafeText unsafeText = *a.m_Data;
				int length = unsafeText.Length;
				int utf8LengthInBytes = (int)b.utf8LengthInBytes;
				byte* unsafePtr = unsafeText.GetUnsafePtr();
				byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes30>(in b.bytes);
				return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
			}

			public static bool operator !=(in NativeText.ReadOnly a, in FixedString32Bytes b)
			{
				return !((in a) == (in b));
			}

			public bool Equals(FixedString32Bytes other)
			{
				return (in this) == (in other);
			}

			public int CompareTo(FixedString64Bytes other)
			{
				return (ref this).CompareTo<NativeText.ReadOnly, FixedString64Bytes>(in other);
			}

			public unsafe static bool operator ==(in NativeText.ReadOnly a, in FixedString64Bytes b)
			{
				UnsafeText unsafeText = *a.m_Data;
				int length = unsafeText.Length;
				int utf8LengthInBytes = (int)b.utf8LengthInBytes;
				byte* unsafePtr = unsafeText.GetUnsafePtr();
				byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes62>(in b.bytes);
				return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
			}

			public static bool operator !=(in NativeText.ReadOnly a, in FixedString64Bytes b)
			{
				return !((in a) == (in b));
			}

			public bool Equals(FixedString64Bytes other)
			{
				return (in this) == (in other);
			}

			public int CompareTo(FixedString128Bytes other)
			{
				return (ref this).CompareTo<NativeText.ReadOnly, FixedString128Bytes>(in other);
			}

			public unsafe static bool operator ==(in NativeText.ReadOnly a, in FixedString128Bytes b)
			{
				UnsafeText unsafeText = *a.m_Data;
				int length = unsafeText.Length;
				int utf8LengthInBytes = (int)b.utf8LengthInBytes;
				byte* unsafePtr = unsafeText.GetUnsafePtr();
				byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes126>(in b.bytes);
				return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
			}

			public static bool operator !=(in NativeText.ReadOnly a, in FixedString128Bytes b)
			{
				return !((in a) == (in b));
			}

			public bool Equals(FixedString128Bytes other)
			{
				return (in this) == (in other);
			}

			public int CompareTo(FixedString512Bytes other)
			{
				return (ref this).CompareTo<NativeText.ReadOnly, FixedString512Bytes>(in other);
			}

			public unsafe static bool operator ==(in NativeText.ReadOnly a, in FixedString512Bytes b)
			{
				UnsafeText unsafeText = *a.m_Data;
				int length = unsafeText.Length;
				int utf8LengthInBytes = (int)b.utf8LengthInBytes;
				byte* unsafePtr = unsafeText.GetUnsafePtr();
				byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes510>(in b.bytes);
				return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
			}

			public static bool operator !=(in NativeText.ReadOnly a, in FixedString512Bytes b)
			{
				return !((in a) == (in b));
			}

			public bool Equals(FixedString512Bytes other)
			{
				return (in this) == (in other);
			}

			public int CompareTo(FixedString4096Bytes other)
			{
				return (ref this).CompareTo<NativeText.ReadOnly, FixedString4096Bytes>(in other);
			}

			public unsafe static bool operator ==(in NativeText.ReadOnly a, in FixedString4096Bytes b)
			{
				UnsafeText unsafeText = *a.m_Data;
				int length = unsafeText.Length;
				int utf8LengthInBytes = (int)b.utf8LengthInBytes;
				byte* unsafePtr = unsafeText.GetUnsafePtr();
				byte* ptr = (byte*)UnsafeUtilityExtensions.AddressOf<FixedBytes4094>(in b.bytes);
				return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, ptr, utf8LengthInBytes);
			}

			public static bool operator !=(in NativeText.ReadOnly a, in FixedString4096Bytes b)
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
				if (this.m_Data == null)
				{
					return "";
				}
				return (ref this).ConvertToString<NativeText.ReadOnly>();
			}

			public override int GetHashCode()
			{
				return (ref this).ComputeHashCode<NativeText.ReadOnly>();
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
				if (other is NativeText)
				{
					NativeText nativeText = (NativeText)other;
					return this.Equals(nativeText);
				}
				if (other is NativeText.ReadOnly)
				{
					NativeText.ReadOnly readOnly = (NativeText.ReadOnly)other;
					return this.Equals(readOnly);
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

			public NativeText.Enumerator GetEnumerator()
			{
				return new NativeText.Enumerator(this);
			}

			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeText* m_Data;
		}
	}
}
