using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Hierarchy
{
	[DebuggerTypeProxy(typeof(UnsafeCircularBufferTDebugView<>))]
	[DebuggerDisplay("Count = {Count}, Capacity = {Capacity}, IsEmpty = {IsEmpty}")]
	internal class CircularBuffer<T>
	{
		public int Capacity
		{
			get
			{
				return this.m_Capacity;
			}
			set
			{
				this.EnsureCapacity(value);
			}
		}

		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.m_Count == 0;
			}
		}

		public int FrontIndex
		{
			get
			{
				return this.m_Front;
			}
		}

		public int BackIndex
		{
			get
			{
				return this.m_Back;
			}
		}

		public bool Locked
		{
			get
			{
				return this.m_Locked;
			}
			set
			{
				this.m_Locked = value;
			}
		}

		public CircularBuffer()
		{
			this.m_Buffer = Array.Empty<T>();
		}

		public CircularBuffer(int initialCapacity)
		{
			this.EnsureCapacity(initialCapacity);
		}

		public CircularBuffer(T[] items)
		{
			this.EnsureCapacity(items.Length);
			Array.Copy(items, this.m_Buffer, items.Length);
			this.m_Count = items.Length;
		}

		public T this[int index]
		{
			get
			{
				return this.m_Buffer[this.GetIndex(index)];
			}
			set
			{
				this.m_Buffer[this.GetIndex(index)] = value;
			}
		}

		public T Front()
		{
			this.ThrowIfEmpty();
			return this.m_Buffer[this.m_Front];
		}

		public T Back()
		{
			this.ThrowIfEmpty();
			return this.m_Buffer[((this.m_Back == 0) ? this.m_Capacity : this.m_Back) - 1];
		}

		public void PushFront(in T item)
		{
			this.ThrowIfLocked();
			this.EnsureCapacity(this.m_Count + 1);
			this.m_Front = this.Modulo(this.m_Front - 1, this.m_Capacity);
			this.m_Buffer[this.m_Front] = item;
			this.m_Count++;
		}

		public void PushBack(in T item)
		{
			this.ThrowIfLocked();
			this.EnsureCapacity(this.m_Count + 1);
			this.m_Buffer[this.m_Back] = item;
			this.m_Back = this.Modulo(this.m_Back + 1, this.m_Capacity);
			this.m_Count++;
		}

		public void PopFront()
		{
			this.PopFront(1);
		}

		private void PopFront(int count)
		{
			this.ThrowIfLocked();
			this.ThrowIfEmpty();
			count = Math.Min(count, this.m_Count);
			IDisposable disposable = this.m_Buffer[this.m_Front] as IDisposable;
			bool flag = disposable != null;
			if (flag)
			{
				disposable.Dispose();
			}
			this.m_Buffer[this.m_Front] = default(T);
			this.m_Front = this.Modulo(this.m_Front + count, this.m_Capacity);
			this.m_Count -= count;
		}

		public void PopBack()
		{
			this.PopBack(1);
		}

		private void PopBack(int count)
		{
			this.ThrowIfLocked();
			this.ThrowIfEmpty();
			count = Math.Min(count, this.m_Count);
			IDisposable disposable = this.m_Buffer[this.m_Back] as IDisposable;
			bool flag = disposable != null;
			if (flag)
			{
				disposable.Dispose();
			}
			this.m_Buffer[this.m_Back] = default(T);
			this.m_Back = this.Modulo(this.m_Back - count, this.m_Capacity);
			this.m_Count -= count;
		}

		public void Clear()
		{
			this.ThrowIfLocked();
			for (int i = 0; i < this.m_Buffer.Length; i++)
			{
				IDisposable disposable = this.m_Buffer[i] as IDisposable;
				bool flag = disposable != null;
				if (flag)
				{
					disposable.Dispose();
				}
				this.m_Buffer[i] = default(T);
			}
			this.m_Count = 0;
			this.m_Back = 0;
			this.m_Front = 0;
		}

		public CircularBuffer<T>.Enumerator GetEnumerator()
		{
			return new CircularBuffer<T>.Enumerator(this);
		}

		public T[] ToArray()
		{
			T[] array = new T[this.m_Count];
			for (int i = 0; i < this.m_Count; i++)
			{
				array[i] = this.m_Buffer[this.GetIndex(i)];
			}
			return array;
		}

		private void Allocate(int capacity)
		{
			T[] array = new T[capacity];
			bool flag = this.m_Count > 0;
			if (flag)
			{
				for (int i = 0; i < this.m_Count; i++)
				{
					array[i] = this.m_Buffer[this.GetIndex(i)];
				}
				this.m_Front = 0;
				this.m_Back = this.m_Count;
			}
			this.m_Buffer = array;
			this.m_Capacity = capacity;
		}

		private void EnsureCapacity(int capacity)
		{
			bool flag = capacity <= 0;
			if (flag)
			{
				throw new ArgumentException("capacity must be greater than zero.");
			}
			int num = Mathf.NextPowerOfTwo(capacity);
			bool flag2 = num <= this.m_Capacity;
			if (!flag2)
			{
				this.Allocate(num);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetIndex(int index)
		{
			this.ThrowIfIndexOutOfRange(index);
			return this.m_Front + ((index < this.m_Capacity - this.m_Front) ? index : (index - this.m_Capacity));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int Modulo(int x, int y)
		{
			int num = x % y;
			return (num < 0) ? (num + y) : num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfEmpty()
		{
			bool isEmpty = this.IsEmpty;
			if (isEmpty)
			{
				throw new InvalidOperationException("Buffer is empty.");
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfIndexOutOfRange(int index)
		{
			bool isEmpty = this.IsEmpty;
			if (isEmpty)
			{
				throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty.", index));
			}
			bool flag = index < 0;
			if (flag)
			{
				throw new IndexOutOfRangeException(string.Format("Cannot access index {0}.", index));
			}
			bool flag2 = index >= this.m_Count;
			if (flag2)
			{
				throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer count is {1}.", index, this.m_Count));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfLocked()
		{
			bool locked = this.m_Locked;
			if (locked)
			{
				throw new InvalidOperationException("Buffer is locked.");
			}
		}

		private T[] m_Buffer;

		private int m_Front;

		private int m_Back;

		private int m_Capacity;

		private int m_Count;

		private bool m_Locked;

		public struct Enumerator
		{
			public T Current
			{
				get
				{
					return this.m_Buffer[this.m_Index];
				}
			}

			internal Enumerator(CircularBuffer<T> buffer)
			{
				this.m_Buffer = buffer;
				this.m_Index = -1;
			}

			public bool MoveNext()
			{
				int num = this.m_Index + 1;
				this.m_Index = num;
				return num < this.m_Buffer.m_Count;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			private readonly CircularBuffer<T> m_Buffer;

			private int m_Index;
		}
	}
}
