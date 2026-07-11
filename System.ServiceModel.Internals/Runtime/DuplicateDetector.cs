using System;
using System.Collections.Generic;

namespace System.Runtime
{
	internal class DuplicateDetector<T> where T : class
	{
		public DuplicateDetector(int capacity)
		{
			this.capacity = capacity;
			this.items = new Dictionary<T, LinkedListNode<T>>();
			this.fifoList = new LinkedList<T>();
			this.thisLock = new object();
		}

		public bool AddIfNotDuplicate(T value)
		{
			bool flag = false;
			object obj = this.thisLock;
			lock (obj)
			{
				if (!this.items.ContainsKey(value))
				{
					this.Add(value);
					flag = true;
				}
			}
			return flag;
		}

		private void Add(T value)
		{
			if (this.items.Count == this.capacity)
			{
				LinkedListNode<T> last = this.fifoList.Last;
				this.items.Remove(last.Value);
				this.fifoList.Remove(last);
			}
			this.items.Add(value, this.fifoList.AddFirst(value));
		}

		public bool Remove(T value)
		{
			bool flag = false;
			object obj = this.thisLock;
			lock (obj)
			{
				LinkedListNode<T> linkedListNode;
				if (this.items.TryGetValue(value, out linkedListNode))
				{
					this.items.Remove(value);
					this.fifoList.Remove(linkedListNode);
					flag = true;
				}
			}
			return flag;
		}

		public void Clear()
		{
			object obj = this.thisLock;
			lock (obj)
			{
				this.fifoList.Clear();
				this.items.Clear();
			}
		}

		private LinkedList<T> fifoList;

		private Dictionary<T, LinkedListNode<T>> items;

		private int capacity;

		private object thisLock;
	}
}
