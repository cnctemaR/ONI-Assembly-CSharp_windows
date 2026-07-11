using System;
using System.Collections.Generic;

namespace MS.Internal.Xml.XPath
{
	internal sealed class ClonableStack<T> : List<T>
	{
		public ClonableStack()
		{
		}

		public ClonableStack(int capacity)
			: base(capacity)
		{
		}

		private ClonableStack(IEnumerable<T> collection)
			: base(collection)
		{
		}

		public void Push(T value)
		{
			base.Add(value);
		}

		public T Pop()
		{
			int num = base.Count - 1;
			T t = base[num];
			base.RemoveAt(num);
			return t;
		}

		public T Peek()
		{
			return base[base.Count - 1];
		}

		public ClonableStack<T> Clone()
		{
			return new ClonableStack<T>(this);
		}
	}
}
