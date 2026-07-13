using System;

namespace LibTessDotNet
{
	internal class PriorityHeap<TValue> where TValue : class
	{
		public bool Empty
		{
			get
			{
				return this._size == 0;
			}
		}

		public PriorityHeap(int initialSize, PriorityHeap<TValue>.LessOrEqual leq)
		{
			this._leq = leq;
			this._nodes = new int[initialSize + 1];
			this._handles = new PriorityHeap<TValue>.HandleElem[initialSize + 1];
			this._size = 0;
			this._max = initialSize;
			this._freeList = 0;
			this._initialized = false;
			this._nodes[1] = 1;
			this._handles[1] = new PriorityHeap<TValue>.HandleElem
			{
				_key = default(TValue)
			};
		}

		private void FloatDown(int curr)
		{
			int num = this._nodes[curr];
			for (;;)
			{
				int num2 = curr << 1;
				bool flag = num2 < this._size && this._leq(this._handles[this._nodes[num2 + 1]]._key, this._handles[this._nodes[num2]]._key);
				if (flag)
				{
					num2++;
				}
				int num3 = this._nodes[num2];
				bool flag2 = num2 > this._size || this._leq(this._handles[num]._key, this._handles[num3]._key);
				if (flag2)
				{
					break;
				}
				this._nodes[curr] = num3;
				this._handles[num3]._node = curr;
				curr = num2;
			}
			this._nodes[curr] = num;
			this._handles[num]._node = curr;
		}

		private void FloatUp(int curr)
		{
			int num = this._nodes[curr];
			for (;;)
			{
				int num2 = curr >> 1;
				int num3 = this._nodes[num2];
				bool flag = num2 == 0 || this._leq(this._handles[num3]._key, this._handles[num]._key);
				if (flag)
				{
					break;
				}
				this._nodes[curr] = num3;
				this._handles[num3]._node = curr;
				curr = num2;
			}
			this._nodes[curr] = num;
			this._handles[num]._node = curr;
		}

		public void Init()
		{
			for (int i = this._size; i >= 1; i--)
			{
				this.FloatDown(i);
			}
			this._initialized = true;
		}

		public PQHandle Insert(TValue value)
		{
			int num = this._size + 1;
			this._size = num;
			int num2 = num;
			bool flag = num2 * 2 > this._max;
			if (flag)
			{
				this._max <<= 1;
				Array.Resize<int>(ref this._nodes, this._max + 1);
				Array.Resize<PriorityHeap<TValue>.HandleElem>(ref this._handles, this._max + 1);
			}
			bool flag2 = this._freeList == 0;
			int num3;
			if (flag2)
			{
				num3 = num2;
			}
			else
			{
				num3 = this._freeList;
				this._freeList = this._handles[num3]._node;
			}
			this._nodes[num2] = num3;
			bool flag3 = this._handles[num3] == null;
			if (flag3)
			{
				this._handles[num3] = new PriorityHeap<TValue>.HandleElem
				{
					_key = value,
					_node = num2
				};
			}
			else
			{
				this._handles[num3]._node = num2;
				this._handles[num3]._key = value;
			}
			bool initialized = this._initialized;
			if (initialized)
			{
				this.FloatUp(num2);
			}
			return new PQHandle
			{
				_handle = num3
			};
		}

		public TValue ExtractMin()
		{
			int num = this._nodes[1];
			TValue key = this._handles[num]._key;
			bool flag = this._size > 0;
			if (flag)
			{
				this._nodes[1] = this._nodes[this._size];
				this._handles[this._nodes[1]]._node = 1;
				this._handles[num]._key = default(TValue);
				this._handles[num]._node = this._freeList;
				this._freeList = num;
				int num2 = this._size - 1;
				this._size = num2;
				bool flag2 = num2 > 0;
				if (flag2)
				{
					this.FloatDown(1);
				}
			}
			return key;
		}

		public TValue Minimum()
		{
			return this._handles[this._nodes[1]]._key;
		}

		public void Remove(PQHandle handle)
		{
			int handle2 = handle._handle;
			int node = this._handles[handle2]._node;
			this._nodes[node] = this._nodes[this._size];
			this._handles[this._nodes[node]]._node = node;
			int num = node;
			int num2 = this._size - 1;
			this._size = num2;
			bool flag = num <= num2;
			if (flag)
			{
				bool flag2 = node <= 1 || this._leq(this._handles[this._nodes[node >> 1]]._key, this._handles[this._nodes[node]]._key);
				if (flag2)
				{
					this.FloatDown(node);
				}
				else
				{
					this.FloatUp(node);
				}
			}
			this._handles[handle2]._key = default(TValue);
			this._handles[handle2]._node = this._freeList;
			this._freeList = handle2;
		}

		private PriorityHeap<TValue>.LessOrEqual _leq;

		private int[] _nodes;

		private PriorityHeap<TValue>.HandleElem[] _handles;

		private int _size;

		private int _max;

		private int _freeList;

		private bool _initialized;

		public delegate bool LessOrEqual(TValue lhs, TValue rhs);

		protected class HandleElem
		{
			internal TValue _key;

			internal int _node;
		}
	}
}
