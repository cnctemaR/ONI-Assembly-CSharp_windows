using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Collections
{
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(FixedBytes30)
	})]
	[Serializable]
	internal struct FixedList<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U> : INativeList<T>, IIndexable<T> where T : struct, ValueType where U : struct, ValueType
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
				fixed (U* ptr = &this.buffer)
				{
					return (byte*)ptr + FixedList.PaddingBytes<T>();
				}
			}
		}

		public int Capacity
		{
			get
			{
				return FixedList.Capacity<U, T>();
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

		[SerializeField]
		internal ushort length;

		[SerializeField]
		internal U buffer;
	}
}
