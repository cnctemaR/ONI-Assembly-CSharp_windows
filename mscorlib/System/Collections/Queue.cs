using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[DebuggerDisplay("Count={Count}")]
	[ComVisible(true)]
	[DebuggerTypeProxy(typeof(CollectionDebuggerView))]
	[Serializable]
	public class Queue : IEnumerable, ICloneable, ICollection
	{
		public Queue()
			: this(32, 2f)
		{
		}

		public Queue(int capacity)
			: this(capacity, 2f)
		{
		}

		public Queue(ICollection col)
			: this((col != null) ? col.Count : 32)
		{
			if (col == null)
			{
				throw new ArgumentNullException("col");
			}
			foreach (object obj in col)
			{
				this.Enqueue(obj);
			}
		}

		public Queue(int capacity, float growFactor)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", "Needs a non-negative number");
			}
			if (growFactor < 1f || growFactor > 10f)
			{
				throw new ArgumentOutOfRangeException("growFactor", "Queue growth factor must be between 1.0 and 10.0, inclusive");
			}
			this._array = new object[capacity];
			this._growFactor = (int)(growFactor * 100f);
		}

		public virtual int Count
		{
			get
			{
				return this._size;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public virtual void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (array.Rank > 1 || (index != 0 && index >= array.Length) || this._size > array.Length - index)
			{
				throw new ArgumentException();
			}
			int num = this._array.Length;
			int num2 = num - this._head;
			Array.Copy(this._array, this._head, array, index, Math.Min(this._size, num2));
			if (this._size > num2)
			{
				Array.Copy(this._array, 0, array, index + num2, this._size - num2);
			}
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new Queue.QueueEnumerator(this);
		}

		public virtual object Clone()
		{
			Queue queue = new Queue(this._array.Length);
			queue._growFactor = this._growFactor;
			Array.Copy(this._array, 0, queue._array, 0, this._array.Length);
			queue._head = this._head;
			queue._size = this._size;
			queue._tail = this._tail;
			return queue;
		}

		public virtual void Clear()
		{
			this._version++;
			this._head = 0;
			this._size = 0;
			this._tail = 0;
			for (int i = this._array.Length - 1; i >= 0; i--)
			{
				this._array[i] = null;
			}
		}

		public virtual bool Contains(object obj)
		{
			int num = this._head + this._size;
			if (obj == null)
			{
				for (int i = this._head; i < num; i++)
				{
					if (this._array[i % this._array.Length] == null)
					{
						return true;
					}
				}
			}
			else
			{
				for (int j = this._head; j < num; j++)
				{
					if (obj.Equals(this._array[j % this._array.Length]))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual object Dequeue()
		{
			this._version++;
			if (this._size < 1)
			{
				throw new InvalidOperationException();
			}
			object obj = this._array[this._head];
			this._array[this._head] = null;
			this._head = (this._head + 1) % this._array.Length;
			this._size--;
			return obj;
		}

		public virtual void Enqueue(object obj)
		{
			this._version++;
			if (this._size == this._array.Length)
			{
				this.grow();
			}
			this._array[this._tail] = obj;
			this._tail = (this._tail + 1) % this._array.Length;
			this._size++;
		}

		public virtual object Peek()
		{
			if (this._size < 1)
			{
				throw new InvalidOperationException();
			}
			return this._array[this._head];
		}

		public static Queue Synchronized(Queue queue)
		{
			if (queue == null)
			{
				throw new ArgumentNullException("queue");
			}
			return new Queue.SyncQueue(queue);
		}

		public virtual object[] ToArray()
		{
			object[] array = new object[this._size];
			this.CopyTo(array, 0);
			return array;
		}

		public virtual void TrimToSize()
		{
			this._version++;
			object[] array = new object[this._size];
			this.CopyTo(array, 0);
			this._array = array;
			this._head = 0;
			this._tail = 0;
		}

		private void grow()
		{
			int num = this._array.Length * this._growFactor / 100;
			if (num < this._array.Length + 1)
			{
				num = this._array.Length + 1;
			}
			object[] array = new object[num];
			this.CopyTo(array, 0);
			this._array = array;
			this._head = 0;
			this._tail = this._head + this._size;
		}

		private object[] _array;

		private int _head;

		private int _size;

		private int _tail;

		private int _growFactor;

		private int _version;

		private class SyncQueue : Queue
		{
			internal SyncQueue(Queue queue)
			{
				this.queue = queue;
			}

			public override int Count
			{
				get
				{
					Queue queue = this.queue;
					int count;
					lock (queue)
					{
						count = this.queue.Count;
					}
					return count;
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return true;
				}
			}

			public override object SyncRoot
			{
				get
				{
					return this.queue.SyncRoot;
				}
			}

			public override void CopyTo(Array array, int index)
			{
				Queue queue = this.queue;
				lock (queue)
				{
					this.queue.CopyTo(array, index);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				Queue queue = this.queue;
				IEnumerator enumerator;
				lock (queue)
				{
					enumerator = this.queue.GetEnumerator();
				}
				return enumerator;
			}

			public override object Clone()
			{
				Queue queue = this.queue;
				object obj;
				lock (queue)
				{
					obj = new Queue.SyncQueue((Queue)this.queue.Clone());
				}
				return obj;
			}

			public override void Clear()
			{
				Queue queue = this.queue;
				lock (queue)
				{
					this.queue.Clear();
				}
			}

			public override void TrimToSize()
			{
				Queue queue = this.queue;
				lock (queue)
				{
					this.queue.TrimToSize();
				}
			}

			public override bool Contains(object obj)
			{
				Queue queue = this.queue;
				bool flag;
				lock (queue)
				{
					flag = this.queue.Contains(obj);
				}
				return flag;
			}

			public override object Dequeue()
			{
				Queue queue = this.queue;
				object obj;
				lock (queue)
				{
					obj = this.queue.Dequeue();
				}
				return obj;
			}

			public override void Enqueue(object obj)
			{
				Queue queue = this.queue;
				lock (queue)
				{
					this.queue.Enqueue(obj);
				}
			}

			public override object Peek()
			{
				Queue queue = this.queue;
				object obj;
				lock (queue)
				{
					obj = this.queue.Peek();
				}
				return obj;
			}

			public override object[] ToArray()
			{
				Queue queue = this.queue;
				object[] array;
				lock (queue)
				{
					array = this.queue.ToArray();
				}
				return array;
			}

			private Queue queue;
		}

		[Serializable]
		private class QueueEnumerator : IEnumerator, ICloneable
		{
			internal QueueEnumerator(Queue q)
			{
				this.queue = q;
				this._version = q._version;
				this.current = -1;
			}

			public object Clone()
			{
				return new Queue.QueueEnumerator(this.queue)
				{
					_version = this._version,
					current = this.current
				};
			}

			public virtual object Current
			{
				get
				{
					if (this._version != this.queue._version || this.current < 0 || this.current >= this.queue._size)
					{
						throw new InvalidOperationException();
					}
					return this.queue._array[(this.queue._head + this.current) % this.queue._array.Length];
				}
			}

			public virtual bool MoveNext()
			{
				if (this._version != this.queue._version)
				{
					throw new InvalidOperationException();
				}
				if (this.current >= this.queue._size - 1)
				{
					this.current = int.MaxValue;
					return false;
				}
				this.current++;
				return true;
			}

			public virtual void Reset()
			{
				if (this._version != this.queue._version)
				{
					throw new InvalidOperationException();
				}
				this.current = -1;
			}

			private Queue queue;

			private int _version;

			private int current;
		}
	}
}
