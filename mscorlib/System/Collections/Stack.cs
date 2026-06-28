using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[DebuggerTypeProxy(typeof(CollectionDebuggerView))]
	[DebuggerDisplay("Count={Count}")]
	[Serializable]
	public class Stack : IEnumerable, ICloneable, ICollection
	{
		public Stack()
		{
			this.contents = new object[16];
			this.capacity = 16;
		}

		public Stack(ICollection col)
			: this((col != null) ? col.Count : 16)
		{
			if (col == null)
			{
				throw new ArgumentNullException("col");
			}
			foreach (object obj in col)
			{
				this.Push(obj);
			}
		}

		public Stack(int initialCapacity)
		{
			if (initialCapacity < 0)
			{
				throw new ArgumentOutOfRangeException("initialCapacity");
			}
			this.capacity = initialCapacity;
			this.contents = new object[this.capacity];
		}

		private void Resize(int ncapacity)
		{
			ncapacity = Math.Max(ncapacity, 16);
			object[] array = new object[ncapacity];
			Array.Copy(this.contents, array, this.count);
			this.capacity = ncapacity;
			this.contents = array;
		}

		public static Stack Synchronized(Stack stack)
		{
			if (stack == null)
			{
				throw new ArgumentNullException("stack");
			}
			return new Stack.SyncStack(stack);
		}

		public virtual int Count
		{
			get
			{
				return this.count;
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

		public virtual void Clear()
		{
			this.modCount++;
			for (int i = 0; i < this.count; i++)
			{
				this.contents[i] = null;
			}
			this.count = 0;
			this.current = -1;
		}

		public virtual object Clone()
		{
			return new Stack(this.contents)
			{
				current = this.current,
				count = this.count
			};
		}

		public virtual bool Contains(object obj)
		{
			if (this.count == 0)
			{
				return false;
			}
			if (obj == null)
			{
				for (int i = 0; i < this.count; i++)
				{
					if (this.contents[i] == null)
					{
						return true;
					}
				}
			}
			else
			{
				for (int j = 0; j < this.count; j++)
				{
					if (obj.Equals(this.contents[j]))
					{
						return true;
					}
				}
			}
			return false;
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
			if (array.Rank > 1 || (array.Length > 0 && index >= array.Length) || this.count > array.Length - index)
			{
				throw new ArgumentException();
			}
			for (int num = this.current; num != -1; num--)
			{
				array.SetValue(this.contents[num], this.count - (num + 1) + index);
			}
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new Stack.Enumerator(this);
		}

		public virtual object Peek()
		{
			if (this.current == -1)
			{
				throw new InvalidOperationException();
			}
			return this.contents[this.current];
		}

		public virtual object Pop()
		{
			if (this.current == -1)
			{
				throw new InvalidOperationException();
			}
			this.modCount++;
			object obj = this.contents[this.current];
			this.contents[this.current] = null;
			this.count--;
			this.current--;
			if (this.count <= this.capacity / 4 && this.count > 16)
			{
				this.Resize(this.capacity / 2);
			}
			return obj;
		}

		public virtual void Push(object obj)
		{
			this.modCount++;
			if (this.capacity == this.count)
			{
				this.Resize(this.capacity * 2);
			}
			this.count++;
			this.current++;
			this.contents[this.current] = obj;
		}

		public virtual object[] ToArray()
		{
			object[] array = new object[this.count];
			Array.Copy(this.contents, array, this.count);
			Array.Reverse(array);
			return array;
		}

		private const int default_capacity = 16;

		private object[] contents;

		private int current = -1;

		private int count;

		private int capacity;

		private int modCount;

		[Serializable]
		private class SyncStack : Stack
		{
			internal SyncStack(Stack s)
			{
				this.stack = s;
			}

			public override int Count
			{
				get
				{
					Stack stack = this.stack;
					int count;
					lock (stack)
					{
						count = this.stack.Count;
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
					return this.stack.SyncRoot;
				}
			}

			public override void Clear()
			{
				Stack stack = this.stack;
				lock (stack)
				{
					this.stack.Clear();
				}
			}

			public override object Clone()
			{
				Stack stack = this.stack;
				object obj;
				lock (stack)
				{
					obj = Stack.Synchronized((Stack)this.stack.Clone());
				}
				return obj;
			}

			public override bool Contains(object obj)
			{
				Stack stack = this.stack;
				bool flag;
				lock (stack)
				{
					flag = this.stack.Contains(obj);
				}
				return flag;
			}

			public override void CopyTo(Array array, int index)
			{
				Stack stack = this.stack;
				lock (stack)
				{
					this.stack.CopyTo(array, index);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				Stack stack = this.stack;
				IEnumerator enumerator;
				lock (stack)
				{
					enumerator = new Stack.Enumerator(this.stack);
				}
				return enumerator;
			}

			public override object Peek()
			{
				Stack stack = this.stack;
				object obj;
				lock (stack)
				{
					obj = this.stack.Peek();
				}
				return obj;
			}

			public override object Pop()
			{
				Stack stack = this.stack;
				object obj;
				lock (stack)
				{
					obj = this.stack.Pop();
				}
				return obj;
			}

			public override void Push(object obj)
			{
				Stack stack = this.stack;
				lock (stack)
				{
					this.stack.Push(obj);
				}
			}

			public override object[] ToArray()
			{
				Stack stack = this.stack;
				object[] array;
				lock (stack)
				{
					array = this.stack.ToArray();
				}
				return array;
			}

			private Stack stack;
		}

		private class Enumerator : IEnumerator, ICloneable
		{
			internal Enumerator(Stack s)
			{
				this.stack = s;
				this.modCount = s.modCount;
				this.current = -2;
			}

			public object Clone()
			{
				return base.MemberwiseClone();
			}

			public virtual object Current
			{
				get
				{
					if (this.modCount != this.stack.modCount || this.current == -2 || this.current == -1 || this.current > this.stack.count)
					{
						throw new InvalidOperationException();
					}
					return this.stack.contents[this.current];
				}
			}

			public virtual bool MoveNext()
			{
				if (this.modCount != this.stack.modCount)
				{
					throw new InvalidOperationException();
				}
				int num = this.current;
				if (num == -2)
				{
					this.current = this.stack.current;
					return this.current != -1;
				}
				if (num != -1)
				{
					this.current--;
					return this.current != -1;
				}
				return false;
			}

			public virtual void Reset()
			{
				if (this.modCount != this.stack.modCount)
				{
					throw new InvalidOperationException();
				}
				this.current = -2;
			}

			private const int EOF = -1;

			private const int BOF = -2;

			private Stack stack;

			private int modCount;

			private int current;
		}
	}
}
