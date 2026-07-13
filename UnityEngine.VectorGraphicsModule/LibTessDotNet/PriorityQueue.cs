using System;
using System.Collections.Generic;

namespace LibTessDotNet
{
	internal class PriorityQueue<TValue> where TValue : class
	{
		public bool Empty
		{
			get
			{
				return this._size == 0 && this._heap.Empty;
			}
		}

		public PriorityQueue(int initialSize, PriorityHeap<TValue>.LessOrEqual leq)
		{
			this._leq = leq;
			this._heap = new PriorityHeap<TValue>(initialSize, leq);
			this._keys = new TValue[initialSize];
			this._size = 0;
			this._max = initialSize;
			this._initialized = false;
		}

		private static void Swap(ref int a, ref int b)
		{
			int num = a;
			a = b;
			b = num;
		}

		public void Init()
		{
			Stack<PriorityQueue<TValue>.StackItem> stack = new Stack<PriorityQueue<TValue>.StackItem>();
			uint num = 2016473283U;
			int num2 = 0;
			int i = this._size - 1;
			this._order = new int[this._size + 1];
			int num3 = 0;
			for (int j = num2; j <= i; j++)
			{
				this._order[j] = num3;
				num3++;
			}
			stack.Push(new PriorityQueue<TValue>.StackItem
			{
				p = num2,
				r = i
			});
			while (stack.Count > 0)
			{
				PriorityQueue<TValue>.StackItem stackItem = stack.Pop();
				num2 = stackItem.p;
				i = stackItem.r;
				while (i > num2 + 10)
				{
					num = num * 1539415821U + 1U;
					int j = num2 + (int)((ulong)num % (ulong)((long)(i - num2 + 1)));
					num3 = this._order[j];
					this._order[j] = this._order[num2];
					this._order[num2] = num3;
					j = num2 - 1;
					int num4 = i + 1;
					do
					{
						do
						{
							j++;
						}
						while (!this._leq(this._keys[this._order[j]], this._keys[num3]));
						do
						{
							num4--;
						}
						while (!this._leq(this._keys[num3], this._keys[this._order[num4]]));
						PriorityQueue<TValue>.Swap(ref this._order[j], ref this._order[num4]);
					}
					while (j < num4);
					PriorityQueue<TValue>.Swap(ref this._order[j], ref this._order[num4]);
					bool flag = j - num2 < i - num4;
					if (flag)
					{
						stack.Push(new PriorityQueue<TValue>.StackItem
						{
							p = num4 + 1,
							r = i
						});
						i = j - 1;
					}
					else
					{
						stack.Push(new PriorityQueue<TValue>.StackItem
						{
							p = num2,
							r = j - 1
						});
						num2 = num4 + 1;
					}
				}
				for (int j = num2 + 1; j <= i; j++)
				{
					num3 = this._order[j];
					int num4 = j;
					while (num4 > num2 && !this._leq(this._keys[num3], this._keys[this._order[num4 - 1]]))
					{
						this._order[num4] = this._order[num4 - 1];
						num4--;
					}
					this._order[num4] = num3;
				}
			}
			this._max = this._size;
			this._initialized = true;
			this._heap.Init();
		}

		public PQHandle Insert(TValue value)
		{
			bool initialized = this._initialized;
			PQHandle pqhandle;
			if (initialized)
			{
				pqhandle = this._heap.Insert(value);
			}
			else
			{
				int size = this._size;
				int num = this._size + 1;
				this._size = num;
				bool flag = num >= this._max;
				if (flag)
				{
					this._max <<= 1;
					Array.Resize<TValue>(ref this._keys, this._max);
				}
				this._keys[size] = value;
				pqhandle = new PQHandle
				{
					_handle = -(size + 1)
				};
			}
			return pqhandle;
		}

		public TValue ExtractMin()
		{
			bool flag = this._size == 0;
			TValue tvalue;
			if (flag)
			{
				tvalue = this._heap.ExtractMin();
			}
			else
			{
				TValue tvalue2 = this._keys[this._order[this._size - 1]];
				bool flag2 = !this._heap.Empty;
				if (flag2)
				{
					TValue tvalue3 = this._heap.Minimum();
					bool flag3 = this._leq(tvalue3, tvalue2);
					if (flag3)
					{
						return this._heap.ExtractMin();
					}
				}
				do
				{
					this._size--;
				}
				while (this._size > 0 && this._keys[this._order[this._size - 1]] == null);
				tvalue = tvalue2;
			}
			return tvalue;
		}

		public TValue Minimum()
		{
			bool flag = this._size == 0;
			TValue tvalue;
			if (flag)
			{
				tvalue = this._heap.Minimum();
			}
			else
			{
				TValue tvalue2 = this._keys[this._order[this._size - 1]];
				bool flag2 = !this._heap.Empty;
				if (flag2)
				{
					TValue tvalue3 = this._heap.Minimum();
					bool flag3 = this._leq(tvalue3, tvalue2);
					if (flag3)
					{
						return tvalue3;
					}
				}
				tvalue = tvalue2;
			}
			return tvalue;
		}

		public void Remove(PQHandle handle)
		{
			int num = handle._handle;
			bool flag = num >= 0;
			if (flag)
			{
				this._heap.Remove(handle);
			}
			else
			{
				num = -(num + 1);
				this._keys[num] = default(TValue);
				while (this._size > 0 && this._keys[this._order[this._size - 1]] == null)
				{
					this._size--;
				}
			}
		}

		private PriorityHeap<TValue>.LessOrEqual _leq;

		private PriorityHeap<TValue> _heap;

		private TValue[] _keys;

		private int[] _order;

		private int _size;

		private int _max;

		private bool _initialized;

		private class StackItem
		{
			internal int p;

			internal int r;
		}
	}
}
