using System;
using System.Diagnostics;

namespace TMPro
{
	[DebuggerDisplay("Item count = {m_Count}")]
	public struct TMP_TextProcessingStack<T>
	{
		public TMP_TextProcessingStack(T[] stack)
		{
			this.itemStack = stack;
			this.m_Capacity = stack.Length;
			this.index = 0;
			this.m_RolloverSize = 0;
			this.m_DefaultItem = default(T);
			this.m_Count = 0;
		}

		public TMP_TextProcessingStack(int capacity)
		{
			this.itemStack = new T[capacity];
			this.m_Capacity = capacity;
			this.index = 0;
			this.m_RolloverSize = 0;
			this.m_DefaultItem = default(T);
			this.m_Count = 0;
		}

		public TMP_TextProcessingStack(int capacity, int rolloverSize)
		{
			this.itemStack = new T[capacity];
			this.m_Capacity = capacity;
			this.index = 0;
			this.m_RolloverSize = rolloverSize;
			this.m_DefaultItem = default(T);
			this.m_Count = 0;
		}

		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		public T current
		{
			get
			{
				if (this.index > 0)
				{
					return this.itemStack[this.index - 1];
				}
				return this.itemStack[0];
			}
		}

		public int rolloverSize
		{
			get
			{
				return this.m_RolloverSize;
			}
			set
			{
				this.m_RolloverSize = value;
			}
		}

		internal static void SetDefault(TMP_TextProcessingStack<T>[] stack, T item)
		{
			for (int i = 0; i < stack.Length; i++)
			{
				stack[i].SetDefault(item);
			}
		}

		public void Clear()
		{
			this.index = 0;
			this.m_Count = 0;
		}

		public void SetDefault(T item)
		{
			if (this.itemStack == null)
			{
				this.m_Capacity = 4;
				this.itemStack = new T[this.m_Capacity];
				this.m_DefaultItem = default(T);
			}
			this.itemStack[0] = item;
			this.index = 1;
			this.m_Count = 1;
		}

		public void Add(T item)
		{
			if (this.index < this.itemStack.Length)
			{
				this.itemStack[this.index] = item;
				this.index++;
			}
		}

		public T Remove()
		{
			this.index--;
			this.m_Count--;
			if (this.index <= 0)
			{
				this.m_Count = 0;
				this.index = 1;
				return this.itemStack[0];
			}
			return this.itemStack[this.index - 1];
		}

		public void Push(T item)
		{
			if (this.index == this.m_Capacity)
			{
				this.m_Capacity *= 2;
				if (this.m_Capacity == 0)
				{
					this.m_Capacity = 4;
				}
				Array.Resize<T>(ref this.itemStack, this.m_Capacity);
			}
			this.itemStack[this.index] = item;
			if (this.m_RolloverSize == 0)
			{
				this.index++;
				this.m_Count++;
				return;
			}
			this.index = (this.index + 1) % this.m_RolloverSize;
			this.m_Count = ((this.m_Count < this.m_RolloverSize) ? (this.m_Count + 1) : this.m_RolloverSize);
		}

		public T Pop()
		{
			if (this.index == 0 && this.m_RolloverSize == 0)
			{
				return default(T);
			}
			if (this.m_RolloverSize == 0)
			{
				this.index--;
			}
			else
			{
				this.index = (this.index - 1) % this.m_RolloverSize;
				this.index = ((this.index < 0) ? (this.index + this.m_RolloverSize) : this.index);
			}
			T t = this.itemStack[this.index];
			this.itemStack[this.index] = this.m_DefaultItem;
			this.m_Count = ((this.m_Count > 0) ? (this.m_Count - 1) : 0);
			return t;
		}

		public T Peek()
		{
			if (this.index == 0)
			{
				return this.m_DefaultItem;
			}
			return this.itemStack[this.index - 1];
		}

		public T CurrentItem()
		{
			if (this.index > 0)
			{
				return this.itemStack[this.index - 1];
			}
			return this.itemStack[0];
		}

		public T PreviousItem()
		{
			if (this.index > 1)
			{
				return this.itemStack[this.index - 2];
			}
			return this.itemStack[0];
		}

		public T[] itemStack;

		public int index;

		private T m_DefaultItem;

		private int m_Capacity;

		private int m_RolloverSize;

		private int m_Count;

		private const int k_DefaultCapacity = 4;
	}
}
