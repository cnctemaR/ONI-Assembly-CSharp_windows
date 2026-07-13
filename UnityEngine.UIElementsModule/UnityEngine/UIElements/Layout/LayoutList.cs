using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements.Layout
{
	internal struct LayoutList<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : IDisposable where T : struct, ValueType
	{
		public unsafe int Count
		{
			get
			{
				return this.m_Data->Count;
			}
		}

		public bool IsCreated
		{
			get
			{
				return null != this.m_Data;
			}
		}

		public unsafe ref T this[int index]
		{
			get
			{
				bool flag = (ulong)index > (ulong)((long)this.m_Data->Count);
				if (flag)
				{
					throw new ArgumentOutOfRangeException();
				}
				return ref this.m_Data->Values[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			}
		}

		public LayoutList()
		{
			this.m_Data = null;
		}

		public unsafe LayoutList(int initialCapacity)
		{
			this.m_Data = (LayoutList<T>.Data*)UnsafeUtility.Malloc((long)UnsafeUtility.SizeOf<LayoutList<T>.Data>(), 16, LayoutList<T>.s_Label);
			Assert.IsTrue(this.m_Data != null);
			UnsafeUtility.MemClear((void*)this.m_Data, (long)UnsafeUtility.SizeOf<LayoutList<T>.Data>());
			this.ResizeCapacity(initialCapacity);
		}

		public unsafe void Dispose()
		{
			bool flag = null == this.m_Data;
			if (!flag)
			{
				bool flag2 = this.m_Data->Values != null;
				if (flag2)
				{
					UnsafeUtility.Free((void*)this.m_Data->Values, LayoutList<T>.s_Label);
				}
				UnsafeUtility.Free((void*)this.m_Data, LayoutList<T>.s_Label);
				this.m_Data = null;
			}
		}

		public unsafe void Insert(int index, T value)
		{
			bool flag = (ulong)index > (ulong)((long)this.m_Data->Count);
			if (flag)
			{
				throw new ArgumentOutOfRangeException();
			}
			bool flag2 = this.m_Data->Capacity == this.m_Data->Count;
			if (flag2)
			{
				this.IncreaseCapacity();
			}
			bool flag3 = index < this.m_Data->Count;
			if (flag3)
			{
				UnsafeUtility.MemMove((void*)(this.m_Data->Values + (IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T) + sizeof(T) / sizeof(T)), (void*)(this.m_Data->Values + (IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)), (long)(UnsafeUtility.SizeOf<T>() * (this.m_Data->Count - index)));
			}
			this.m_Data->Values[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
			this.m_Data->Count++;
		}

		public unsafe int IndexOf(T value)
		{
			int count = this.m_Data->Count;
			T* ptr = &value;
			T* ptr2 = this.m_Data->Values;
			int num = UnsafeUtility.SizeOf<T>();
			int i = 0;
			while (i < count)
			{
				bool flag = UnsafeUtility.MemCmp((void*)ptr2, (void*)ptr, (long)num) == 0;
				if (flag)
				{
					return i;
				}
				i++;
				ptr2 += sizeof(T) / sizeof(T);
			}
			return -1;
		}

		public unsafe void RemoveAt(int index)
		{
			bool flag = (ulong)index >= (ulong)((long)this.m_Data->Count);
			if (flag)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.m_Data->Count--;
			UnsafeUtility.MemMove((void*)(this.m_Data->Values + (IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)), (void*)(this.m_Data->Values + (IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T) + sizeof(T) / sizeof(T)), (long)(UnsafeUtility.SizeOf<T>() * (this.m_Data->Count - index)));
			this.m_Data->Values[(IntPtr)this.m_Data->Count * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = default(T);
		}

		public unsafe void Clear()
		{
			this.m_Data->Count = 0;
		}

		private unsafe void IncreaseCapacity()
		{
			this.EnsureCapacity(this.m_Data->Capacity * 2);
		}

		private unsafe void EnsureCapacity(int capacity)
		{
			bool flag = capacity <= this.m_Data->Capacity;
			if (!flag)
			{
				this.ResizeCapacity(capacity);
			}
		}

		private unsafe void ResizeCapacity(int capacity)
		{
			Assert.IsTrue(capacity > 0);
			this.m_Data->Values = (T*)LayoutList<T>.ResizeArray((void*)this.m_Data->Values, (long)this.m_Data->Capacity, (long)capacity, (long)UnsafeUtility.SizeOf<T>(), 16);
			this.m_Data->Capacity = capacity;
		}

		private unsafe static void* ResizeArray(void* fromPtr, long fromCount, long toCount, long size, int align)
		{
			Assert.IsTrue(toCount > 0L);
			void* ptr = UnsafeUtility.Malloc(size * toCount, align, LayoutList<T>.s_Label);
			Assert.IsTrue(ptr != null);
			bool flag = fromCount <= 0L;
			void* ptr2;
			if (flag)
			{
				ptr2 = ptr;
			}
			else
			{
				long num = ((toCount < fromCount) ? toCount : fromCount);
				long num2 = num * size;
				UnsafeUtility.MemCpy(ptr, fromPtr, num2);
				UnsafeUtility.Free(fromPtr, LayoutList<T>.s_Label);
				ptr2 = ptr;
			}
			return ptr2;
		}

		public LayoutList<T>.Enumerator GetEnumerator()
		{
			return new LayoutList<T>.Enumerator(this);
		}

		private static readonly MemoryLabel s_Label = new MemoryLabel("UIElements", "Layout.LayoutList", Allocator.Persistent);

		private unsafe LayoutList<T>.Data* m_Data;

		private struct Data
		{
			public int Capacity;

			public int Count;

			public unsafe T* Values;
		}

		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			public T Current
			{
				get
				{
					return this.m_Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.m_Current;
				}
			}

			public Enumerator(LayoutList<T> list)
			{
				this.m_List = list;
				this.m_Index = 0;
				this.m_Current = default(T);
			}

			public void Dispose()
			{
			}

			public unsafe bool MoveNext()
			{
				bool flag = !this.m_List.IsCreated;
				bool flag2;
				if (flag)
				{
					this.m_Current = default(T);
					flag2 = false;
				}
				else
				{
					bool flag3 = (ulong)this.m_Index >= (ulong)((long)this.m_List.Count);
					if (flag3)
					{
						this.m_Current = default(T);
						flag2 = false;
					}
					else
					{
						this.m_Current = *this.m_List[this.m_Index];
						this.m_Index++;
						flag2 = true;
					}
				}
				return flag2;
			}

			public void Reset()
			{
				this.m_Index = 0;
			}

			private LayoutList<T> m_List;

			private int m_Index;

			private T m_Current;
		}
	}
}
