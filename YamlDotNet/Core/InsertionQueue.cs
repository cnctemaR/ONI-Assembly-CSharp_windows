using System;
using System.Collections.Generic;

namespace YamlDotNet.Core
{
	[Serializable]
	public class InsertionQueue<T>
	{
		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public void Enqueue(T item)
		{
			this.items.Add(item);
		}

		public T Dequeue()
		{
			if (this.Count == 0)
			{
				throw new InvalidOperationException("The queue is empty");
			}
			T t = this.items[0];
			this.items.RemoveAt(0);
			return t;
		}

		public void Insert(int index, T item)
		{
			this.items.Insert(index, item);
		}

		private readonly IList<T> items = new List<T>();
	}
}
