using System;
using System.Collections.Generic;

namespace YamlDotNet.Core
{
	public class FakeList<T>
	{
		public FakeList(IEnumerator<T> collection)
		{
			this.collection = collection;
		}

		public FakeList(IEnumerable<T> collection)
			: this(collection.GetEnumerator())
		{
		}

		public T this[int index]
		{
			get
			{
				if (index < this.currentIndex)
				{
					this.collection.Reset();
					this.currentIndex = -1;
				}
				while (this.currentIndex < index)
				{
					if (!this.collection.MoveNext())
					{
						throw new ArgumentOutOfRangeException("index");
					}
					this.currentIndex++;
				}
				return this.collection.Current;
			}
		}

		private readonly IEnumerator<T> collection;

		private int currentIndex = -1;
	}
}
