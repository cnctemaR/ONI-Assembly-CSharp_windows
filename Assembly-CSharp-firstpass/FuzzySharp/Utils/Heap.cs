using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FuzzySharp.Utils
{
	public abstract class Heap<T> : IEnumerable<T>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this._tail;
			}
		}

		public int Capacity
		{
			get
			{
				return this._capacity;
			}
		}

		protected Comparer<T> Comparer { get; }

		protected abstract bool Dominates(T x, T y);

		protected Heap()
			: this(Comparer<T>.Default)
		{
		}

		protected Heap(Comparer<T> comparer)
			: this(Enumerable.Empty<T>(), comparer)
		{
		}

		protected Heap(IEnumerable<T> collection)
			: this(collection, Comparer<T>.Default)
		{
		}

		protected Heap(IEnumerable<T> collection, Comparer<T> comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			this.Comparer = comparer;
			foreach (T t in collection)
			{
				if (this.Count == this.Capacity)
				{
					this.Grow();
				}
				T[] heap = this._heap;
				int tail = this._tail;
				this._tail = tail + 1;
				heap[tail] = t;
			}
			for (int i = Heap<T>.Parent(this._tail - 1); i >= 0; i--)
			{
				this.BubbleDown(i);
			}
		}

		public void Add(T item)
		{
			if (this.Count == this.Capacity)
			{
				this.Grow();
			}
			T[] heap = this._heap;
			int tail = this._tail;
			this._tail = tail + 1;
			heap[tail] = item;
			this.BubbleUp(this._tail - 1);
		}

		private void BubbleUp(int i)
		{
			while (i != 0 && !this.Dominates(this._heap[Heap<T>.Parent(i)], this._heap[i]))
			{
				this.Swap(i, Heap<T>.Parent(i));
				i = Heap<T>.Parent(i);
			}
		}

		public T GetMin()
		{
			if (this.Count == 0)
			{
				throw new InvalidOperationException("Heap is empty");
			}
			return this._heap[0];
		}

		public T ExtractDominating()
		{
			if (this.Count == 0)
			{
				throw new InvalidOperationException("Heap is empty");
			}
			T t = this._heap[0];
			this._tail--;
			this.Swap(this._tail, 0);
			this.BubbleDown(0);
			return t;
		}

		private void BubbleDown(int i)
		{
			for (;;)
			{
				int num = this.Dominating(i);
				if (num == i)
				{
					break;
				}
				this.Swap(i, num);
				i = num;
			}
		}

		private int Dominating(int i)
		{
			int dominating = this.GetDominating(Heap<T>.YoungChild(i), i);
			return this.GetDominating(Heap<T>.OldChild(i), dominating);
		}

		private int GetDominating(int newNode, int dominatingNode)
		{
			if (newNode < this._tail && !this.Dominates(this._heap[dominatingNode], this._heap[newNode]))
			{
				return newNode;
			}
			return dominatingNode;
		}

		private void Swap(int i, int j)
		{
			T t = this._heap[i];
			this._heap[i] = this._heap[j];
			this._heap[j] = t;
		}

		private static int Parent(int i)
		{
			return (i + 1) / 2 - 1;
		}

		private static int YoungChild(int i)
		{
			return (i + 1) * 2 - 1;
		}

		private static int OldChild(int i)
		{
			return Heap<T>.YoungChild(i) + 1;
		}

		private void Grow()
		{
			int num = this._capacity * 2 + 1;
			T[] array = new T[num];
			Array.Copy(this._heap, array, this._capacity);
			this._heap = array;
			this._capacity = num;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this._heap.Take<T>(this.Count).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private const int InitialCapacity = 0;

		private const int GrowFactor = 2;

		private const int MinGrow = 1;

		private int _capacity;

		private T[] _heap = new T[0];

		private int _tail;
	}
}
