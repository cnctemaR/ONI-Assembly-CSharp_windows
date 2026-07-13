using System;

namespace LibTessDotNet
{
	internal class Dict<TValue> where TValue : class
	{
		public Dict(Dict<TValue>.LessOrEqual leq)
		{
			this._leq = leq;
			this._head = new Dict<TValue>.Node
			{
				_key = default(TValue)
			};
			this._head._prev = this._head;
			this._head._next = this._head;
		}

		public Dict<TValue>.Node Insert(TValue key)
		{
			return this.InsertBefore(this._head, key);
		}

		public Dict<TValue>.Node InsertBefore(Dict<TValue>.Node node, TValue key)
		{
			do
			{
				node = node._prev;
			}
			while (node._key != null && !this._leq(node._key, key));
			Dict<TValue>.Node node2 = new Dict<TValue>.Node
			{
				_key = key
			};
			node2._next = node._next;
			node._next._prev = node2;
			node2._prev = node;
			node._next = node2;
			return node2;
		}

		public Dict<TValue>.Node Find(TValue key)
		{
			Dict<TValue>.Node node = this._head;
			do
			{
				node = node._next;
			}
			while (node._key != null && !this._leq(key, node._key));
			return node;
		}

		public Dict<TValue>.Node Min()
		{
			return this._head._next;
		}

		public void Remove(Dict<TValue>.Node node)
		{
			node._next._prev = node._prev;
			node._prev._next = node._next;
		}

		private Dict<TValue>.LessOrEqual _leq;

		private Dict<TValue>.Node _head;

		public class Node
		{
			public TValue Key
			{
				get
				{
					return this._key;
				}
			}

			public Dict<TValue>.Node Prev
			{
				get
				{
					return this._prev;
				}
			}

			public Dict<TValue>.Node Next
			{
				get
				{
					return this._next;
				}
			}

			internal TValue _key;

			internal Dict<TValue>.Node _prev;

			internal Dict<TValue>.Node _next;
		}

		public delegate bool LessOrEqual(TValue lhs, TValue rhs);
	}
}
