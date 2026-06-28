using System;
using System.Collections.Generic;

namespace TMPro
{
	public class FastAction<A, B, C>
	{
		public void Add(Action<A, B, C> rhs)
		{
			if (!this.lookup.ContainsKey(rhs))
			{
				this.lookup[rhs] = this.delegates.AddLast(rhs);
			}
		}

		public void Remove(Action<A, B, C> rhs)
		{
			LinkedListNode<Action<A, B, C>> linkedListNode;
			if (this.lookup.TryGetValue(rhs, out linkedListNode))
			{
				this.lookup.Remove(rhs);
				this.delegates.Remove(linkedListNode);
			}
		}

		public void Call(A a, B b, C c)
		{
			for (LinkedListNode<Action<A, B, C>> linkedListNode = this.delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value(a, b, c);
			}
		}

		private LinkedList<Action<A, B, C>> delegates = new LinkedList<Action<A, B, C>>();

		private Dictionary<Action<A, B, C>, LinkedListNode<Action<A, B, C>>> lookup = new Dictionary<Action<A, B, C>, LinkedListNode<Action<A, B, C>>>();
	}
}
