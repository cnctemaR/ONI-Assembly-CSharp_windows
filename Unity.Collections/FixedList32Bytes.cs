using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Collections
{
	[DebuggerTypeProxy(typeof(FixedList32BytesDebugView<>))]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	[Serializable]
	public struct FixedList32Bytes<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : INativeList<T>, IIndexable<T>, IEnumerable<T>, IEnumerable, IEquatable<FixedList32Bytes<T>>, IComparable<FixedList32Bytes<T>>, IEquatable<FixedList64Bytes<T>>, IComparable<FixedList64Bytes<T>>, IEquatable<FixedList128Bytes<T>>, IComparable<FixedList128Bytes<T>>, IEquatable<FixedList512Bytes<T>>, IComparable<FixedList512Bytes<T>>, IEquatable<FixedList4096Bytes<T>>, IComparable<FixedList4096Bytes<T>> where T : struct, ValueType
	{
		[CreateProperty]
		public int Length
		{
			get
			{
				return (int)this.length;
			}
			set
			{
				this.length = (ushort)value;
			}
		}

		[CreateProperty]
		private IEnumerable<T> Elements
		{
			get
			{
				return this.ToArray();
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Length == 0;
			}
		}

		internal int LengthInBytes
		{
			get
			{
				return this.Length * UnsafeUtility.SizeOf<T>();
			}
		}

		internal unsafe byte* Buffer
		{
			get
			{
				fixed (byte* ptr = &this.buffer.offset0000.byte0000)
				{
					return ptr + FixedList.PaddingBytes<T>();
				}
			}
		}

		public int Capacity
		{
			get
			{
				return FixedList.Capacity<FixedBytes30, T>();
			}
			set
			{
			}
		}

		public unsafe T this[int index]
		{
			get
			{
				return UnsafeUtility.ReadArrayElement<T>((void*)this.Buffer, CollectionHelper.AssumePositive(index));
			}
			set
			{
				UnsafeUtility.WriteArrayElement<T>((void*)this.Buffer, CollectionHelper.AssumePositive(index), value);
			}
		}

		public unsafe ref T ElementAt(int index)
		{
			return UnsafeUtility.ArrayElementAsRef<T>((void*)this.Buffer, index);
		}

		public unsafe override int GetHashCode()
		{
			return (int)CollectionHelper.Hash((void*)this.Buffer, this.LengthInBytes);
		}

		public void Add(in T item)
		{
			int num = this.Length;
			this.Length = num + 1;
			this[num] = item;
		}

		public unsafe void AddRange(void* ptr, int length)
		{
			for (int i = 0; i < length; i++)
			{
				int num = this.Length;
				this.Length = num + 1;
				this[num] = *(T*)((byte*)ptr + (IntPtr)i * (IntPtr)sizeof(T));
			}
		}

		public void AddNoResize(in T item)
		{
			this.Add(in item);
		}

		public unsafe void AddRangeNoResize(void* ptr, int length)
		{
			this.AddRange(ptr, length);
		}

		public void Clear()
		{
			this.Length = 0;
		}

		public unsafe void InsertRangeWithBeginEnd(int begin, int end)
		{
			int num = end - begin;
			if (num < 1)
			{
				return;
			}
			int num2 = (int)this.length - begin;
			this.Length += num;
			if (num2 < 1)
			{
				return;
			}
			int num3 = num2 * UnsafeUtility.SizeOf<T>();
			byte* ptr = this.Buffer;
			byte* ptr2 = ptr + end * UnsafeUtility.SizeOf<T>();
			byte* ptr3 = ptr + begin * UnsafeUtility.SizeOf<T>();
			UnsafeUtility.MemMove((void*)ptr2, (void*)ptr3, (long)num3);
		}

		public void Insert(int index, in T item)
		{
			this.InsertRangeWithBeginEnd(index, index + 1);
			this[index] = item;
		}

		public void RemoveAtSwapBack(int index)
		{
			this.RemoveRangeSwapBack(index, 1);
		}

		public unsafe void RemoveRangeSwapBack(int index, int count)
		{
			if (count > 0)
			{
				int num = math.max(this.Length - count, index + count);
				int num2 = UnsafeUtility.SizeOf<T>();
				void* ptr = (void*)(this.Buffer + index * num2);
				void* ptr2 = (void*)(this.Buffer + num * num2);
				UnsafeUtility.MemCpy(ptr, ptr2, (long)((this.Length - num) * num2));
				this.Length -= count;
			}
		}

		[Obsolete("RemoveRangeSwapBackWithBeginEnd(begin, end) is deprecated, use RemoveRangeSwapBack(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public void RemoveRangeSwapBackWithBeginEnd(int begin, int end)
		{
			this.RemoveRangeSwapBack(begin, end - begin);
		}

		public void RemoveAt(int index)
		{
			this.RemoveRange(index, 1);
		}

		public unsafe void RemoveRange(int index, int count)
		{
			if (count > 0)
			{
				int num = math.min(index + count, this.Length);
				int num2 = UnsafeUtility.SizeOf<T>();
				void* ptr = (void*)(this.Buffer + index * num2);
				void* ptr2 = (void*)(this.Buffer + num * num2);
				UnsafeUtility.MemCpy(ptr, ptr2, (long)((this.Length - num) * num2));
				this.Length -= count;
			}
		}

		[Obsolete("RemoveRangeWithBeginEnd(begin, end) is deprecated, use RemoveRange(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public void RemoveRangeWithBeginEnd(int begin, int end)
		{
			this.RemoveRange(begin, end - begin);
		}

		[NotBurstCompatible]
		public unsafe T[] ToArray()
		{
			T[] array = new T[this.Length];
			byte* ptr = this.Buffer;
			fixed (T[] array2 = array)
			{
				T* ptr2;
				if (array == null || array2.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = &array2[0];
				}
				UnsafeUtility.MemCpy((void*)ptr2, (void*)ptr, (long)this.LengthInBytes);
			}
			return array;
		}

		public unsafe NativeArray<T> ToNativeArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<T> nativeArray = CollectionHelper.CreateNativeArray<T>(this.Length, allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr<T>(), (void*)this.Buffer, (long)this.LengthInBytes);
			return nativeArray;
		}

		public unsafe static bool operator ==(in FixedList32Bytes<T> a, in FixedList32Bytes<T> b)
		{
			if (a.length != b.length)
			{
				return false;
			}
			FixedList32Bytes<T> fixedList32Bytes = a;
			void* ptr = (void*)fixedList32Bytes.Buffer;
			fixedList32Bytes = b;
			void* ptr2 = (void*)fixedList32Bytes.Buffer;
			fixedList32Bytes = a;
			return UnsafeUtility.MemCmp(ptr, ptr2, (long)fixedList32Bytes.LengthInBytes) == 0;
		}

		public static bool operator !=(in FixedList32Bytes<T> a, in FixedList32Bytes<T> b)
		{
			return !((in a) == (in b));
		}

		public unsafe int CompareTo(FixedList32Bytes<T> other)
		{
			fixed (byte* ptr = &this.buffer.offset0000.byte0000)
			{
				byte* ptr2 = ptr;
				byte* ptr3 = &other.buffer.offset0000.byte0000;
				byte* ptr4 = ptr2 + FixedList.PaddingBytes<T>();
				byte* ptr5 = ptr3 + FixedList.PaddingBytes<T>();
				int num = math.min(this.Length, other.Length);
				for (int i = 0; i < num; i++)
				{
					int num2 = UnsafeUtility.MemCmp((void*)(ptr4 + sizeof(T) * i), (void*)(ptr5 + sizeof(T) * i), (long)sizeof(T));
					if (num2 != 0)
					{
						return num2;
					}
				}
				return this.Length.CompareTo(other.Length);
			}
		}

		public bool Equals(FixedList32Bytes<T> other)
		{
			return this.CompareTo(other) == 0;
		}

		public unsafe static bool operator ==(in FixedList32Bytes<T> a, in FixedList64Bytes<T> b)
		{
			if (a.length != b.length)
			{
				return false;
			}
			FixedList32Bytes<T> fixedList32Bytes = a;
			void* ptr = (void*)fixedList32Bytes.Buffer;
			FixedList64Bytes<T> fixedList64Bytes = b;
			void* ptr2 = (void*)fixedList64Bytes.Buffer;
			fixedList32Bytes = a;
			return UnsafeUtility.MemCmp(ptr, ptr2, (long)fixedList32Bytes.LengthInBytes) == 0;
		}

		public static bool operator !=(in FixedList32Bytes<T> a, in FixedList64Bytes<T> b)
		{
			return !((in a) == (in b));
		}

		public unsafe int CompareTo(FixedList64Bytes<T> other)
		{
			fixed (byte* ptr = &this.buffer.offset0000.byte0000)
			{
				byte* ptr2 = ptr;
				byte* ptr3 = &other.buffer.offset0000.byte0000;
				byte* ptr4 = ptr2 + FixedList.PaddingBytes<T>();
				byte* ptr5 = ptr3 + FixedList.PaddingBytes<T>();
				int num = math.min(this.Length, other.Length);
				for (int i = 0; i < num; i++)
				{
					int num2 = UnsafeUtility.MemCmp((void*)(ptr4 + sizeof(T) * i), (void*)(ptr5 + sizeof(T) * i), (long)sizeof(T));
					if (num2 != 0)
					{
						return num2;
					}
				}
				return this.Length.CompareTo(other.Length);
			}
		}

		public bool Equals(FixedList64Bytes<T> other)
		{
			return this.CompareTo(other) == 0;
		}

		public FixedList32Bytes(in FixedList64Bytes<T> other)
		{
			this = default(FixedList32Bytes<T>);
			this.Initialize(in other);
		}

		internal unsafe int Initialize(in FixedList64Bytes<T> other)
		{
			FixedList64Bytes<T> fixedList64Bytes = other;
			if (fixedList64Bytes.Length > this.Capacity)
			{
				return 1;
			}
			this.length = other.length;
			this.buffer = default(FixedBytes30);
			void* ptr = (void*)this.Buffer;
			fixedList64Bytes = other;
			UnsafeUtility.MemCpy(ptr, (void*)fixedList64Bytes.Buffer, (long)this.LengthInBytes);
			return 0;
		}

		public static implicit operator FixedList32Bytes<T>(in FixedList64Bytes<T> other)
		{
			return new FixedList32Bytes<T>(in other);
		}

		public unsafe static bool operator ==(in FixedList32Bytes<T> a, in FixedList128Bytes<T> b)
		{
			if (a.length != b.length)
			{
				return false;
			}
			FixedList32Bytes<T> fixedList32Bytes = a;
			void* ptr = (void*)fixedList32Bytes.Buffer;
			FixedList128Bytes<T> fixedList128Bytes = b;
			void* ptr2 = (void*)fixedList128Bytes.Buffer;
			fixedList32Bytes = a;
			return UnsafeUtility.MemCmp(ptr, ptr2, (long)fixedList32Bytes.LengthInBytes) == 0;
		}

		public static bool operator !=(in FixedList32Bytes<T> a, in FixedList128Bytes<T> b)
		{
			return !((in a) == (in b));
		}

		public unsafe int CompareTo(FixedList128Bytes<T> other)
		{
			fixed (byte* ptr = &this.buffer.offset0000.byte0000)
			{
				byte* ptr2 = ptr;
				byte* ptr3 = &other.buffer.offset0000.byte0000;
				byte* ptr4 = ptr2 + FixedList.PaddingBytes<T>();
				byte* ptr5 = ptr3 + FixedList.PaddingBytes<T>();
				int num = math.min(this.Length, other.Length);
				for (int i = 0; i < num; i++)
				{
					int num2 = UnsafeUtility.MemCmp((void*)(ptr4 + sizeof(T) * i), (void*)(ptr5 + sizeof(T) * i), (long)sizeof(T));
					if (num2 != 0)
					{
						return num2;
					}
				}
				return this.Length.CompareTo(other.Length);
			}
		}

		public bool Equals(FixedList128Bytes<T> other)
		{
			return this.CompareTo(other) == 0;
		}

		public FixedList32Bytes(in FixedList128Bytes<T> other)
		{
			this = default(FixedList32Bytes<T>);
			this.Initialize(in other);
		}

		internal unsafe int Initialize(in FixedList128Bytes<T> other)
		{
			FixedList128Bytes<T> fixedList128Bytes = other;
			if (fixedList128Bytes.Length > this.Capacity)
			{
				return 1;
			}
			this.length = other.length;
			this.buffer = default(FixedBytes30);
			void* ptr = (void*)this.Buffer;
			fixedList128Bytes = other;
			UnsafeUtility.MemCpy(ptr, (void*)fixedList128Bytes.Buffer, (long)this.LengthInBytes);
			return 0;
		}

		public static implicit operator FixedList32Bytes<T>(in FixedList128Bytes<T> other)
		{
			return new FixedList32Bytes<T>(in other);
		}

		public unsafe static bool operator ==(in FixedList32Bytes<T> a, in FixedList512Bytes<T> b)
		{
			if (a.length != b.length)
			{
				return false;
			}
			FixedList32Bytes<T> fixedList32Bytes = a;
			void* ptr = (void*)fixedList32Bytes.Buffer;
			FixedList512Bytes<T> fixedList512Bytes = b;
			void* ptr2 = (void*)fixedList512Bytes.Buffer;
			fixedList32Bytes = a;
			return UnsafeUtility.MemCmp(ptr, ptr2, (long)fixedList32Bytes.LengthInBytes) == 0;
		}

		public static bool operator !=(in FixedList32Bytes<T> a, in FixedList512Bytes<T> b)
		{
			return !((in a) == (in b));
		}

		public unsafe int CompareTo(FixedList512Bytes<T> other)
		{
			fixed (byte* ptr = &this.buffer.offset0000.byte0000)
			{
				byte* ptr2 = ptr;
				byte* ptr3 = &other.buffer.offset0000.byte0000;
				byte* ptr4 = ptr2 + FixedList.PaddingBytes<T>();
				byte* ptr5 = ptr3 + FixedList.PaddingBytes<T>();
				int num = math.min(this.Length, other.Length);
				for (int i = 0; i < num; i++)
				{
					int num2 = UnsafeUtility.MemCmp((void*)(ptr4 + sizeof(T) * i), (void*)(ptr5 + sizeof(T) * i), (long)sizeof(T));
					if (num2 != 0)
					{
						return num2;
					}
				}
				return this.Length.CompareTo(other.Length);
			}
		}

		public bool Equals(FixedList512Bytes<T> other)
		{
			return this.CompareTo(other) == 0;
		}

		public FixedList32Bytes(in FixedList512Bytes<T> other)
		{
			this = default(FixedList32Bytes<T>);
			this.Initialize(in other);
		}

		internal unsafe int Initialize(in FixedList512Bytes<T> other)
		{
			FixedList512Bytes<T> fixedList512Bytes = other;
			if (fixedList512Bytes.Length > this.Capacity)
			{
				return 1;
			}
			this.length = other.length;
			this.buffer = default(FixedBytes30);
			void* ptr = (void*)this.Buffer;
			fixedList512Bytes = other;
			UnsafeUtility.MemCpy(ptr, (void*)fixedList512Bytes.Buffer, (long)this.LengthInBytes);
			return 0;
		}

		public static implicit operator FixedList32Bytes<T>(in FixedList512Bytes<T> other)
		{
			return new FixedList32Bytes<T>(in other);
		}

		public unsafe static bool operator ==(in FixedList32Bytes<T> a, in FixedList4096Bytes<T> b)
		{
			if (a.length != b.length)
			{
				return false;
			}
			FixedList32Bytes<T> fixedList32Bytes = a;
			void* ptr = (void*)fixedList32Bytes.Buffer;
			FixedList4096Bytes<T> fixedList4096Bytes = b;
			void* ptr2 = (void*)fixedList4096Bytes.Buffer;
			fixedList32Bytes = a;
			return UnsafeUtility.MemCmp(ptr, ptr2, (long)fixedList32Bytes.LengthInBytes) == 0;
		}

		public static bool operator !=(in FixedList32Bytes<T> a, in FixedList4096Bytes<T> b)
		{
			return !((in a) == (in b));
		}

		public unsafe int CompareTo(FixedList4096Bytes<T> other)
		{
			fixed (byte* ptr = &this.buffer.offset0000.byte0000)
			{
				byte* ptr2 = ptr;
				byte* ptr3 = &other.buffer.offset0000.byte0000;
				byte* ptr4 = ptr2 + FixedList.PaddingBytes<T>();
				byte* ptr5 = ptr3 + FixedList.PaddingBytes<T>();
				int num = math.min(this.Length, other.Length);
				for (int i = 0; i < num; i++)
				{
					int num2 = UnsafeUtility.MemCmp((void*)(ptr4 + sizeof(T) * i), (void*)(ptr5 + sizeof(T) * i), (long)sizeof(T));
					if (num2 != 0)
					{
						return num2;
					}
				}
				return this.Length.CompareTo(other.Length);
			}
		}

		public bool Equals(FixedList4096Bytes<T> other)
		{
			return this.CompareTo(other) == 0;
		}

		public FixedList32Bytes(in FixedList4096Bytes<T> other)
		{
			this = default(FixedList32Bytes<T>);
			this.Initialize(in other);
		}

		internal unsafe int Initialize(in FixedList4096Bytes<T> other)
		{
			FixedList4096Bytes<T> fixedList4096Bytes = other;
			if (fixedList4096Bytes.Length > this.Capacity)
			{
				return 1;
			}
			this.length = other.length;
			this.buffer = default(FixedBytes30);
			void* ptr = (void*)this.Buffer;
			fixedList4096Bytes = other;
			UnsafeUtility.MemCpy(ptr, (void*)fixedList4096Bytes.Buffer, (long)this.LengthInBytes);
			return 0;
		}

		public static implicit operator FixedList32Bytes<T>(in FixedList4096Bytes<T> other)
		{
			return new FixedList32Bytes<T>(in other);
		}

		[NotBurstCompatible]
		public override bool Equals(object obj)
		{
			if (obj is FixedList32Bytes<T>)
			{
				FixedList32Bytes<T> fixedList32Bytes = (FixedList32Bytes<T>)obj;
				return this.Equals(fixedList32Bytes);
			}
			if (obj is FixedList64Bytes<T>)
			{
				FixedList64Bytes<T> fixedList64Bytes = (FixedList64Bytes<T>)obj;
				return this.Equals(fixedList64Bytes);
			}
			if (obj is FixedList128Bytes<T>)
			{
				FixedList128Bytes<T> fixedList128Bytes = (FixedList128Bytes<T>)obj;
				return this.Equals(fixedList128Bytes);
			}
			if (obj is FixedList512Bytes<T>)
			{
				FixedList512Bytes<T> fixedList512Bytes = (FixedList512Bytes<T>)obj;
				return this.Equals(fixedList512Bytes);
			}
			if (obj is FixedList4096Bytes<T>)
			{
				FixedList4096Bytes<T> fixedList4096Bytes = (FixedList4096Bytes<T>)obj;
				return this.Equals(fixedList4096Bytes);
			}
			return false;
		}

		public FixedList32Bytes<T>.Enumerator GetEnumerator()
		{
			return new FixedList32Bytes<T>.Enumerator(ref this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		[SerializeField]
		internal ushort length;

		[SerializeField]
		internal FixedBytes30 buffer;

		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			public Enumerator(ref FixedList32Bytes<T> list)
			{
				this.m_List = list;
				this.m_Index = -1;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				this.m_Index++;
				return this.m_Index < this.m_List.Length;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			public T Current
			{
				get
				{
					return this.m_List[this.m_Index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private FixedList32Bytes<T> m_List;

			private int m_Index;
		}
	}
}
