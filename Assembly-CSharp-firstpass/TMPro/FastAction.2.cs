using System;
using System.Collections.Generic;

namespace TMPro
{
	public class FastAction<A>
	{
		public void Add(Action<A> rhs)
		{
			if (!this.lookup.ContainsKey(rhs))
			{
				this.lookup[rhs] = this.delegates.AddLast(rhs);
			}
		}

		public void Remove(Action<A> rhs)
		{
			LinkedListNode<Action<A>> linkedListNode;
			if (this.lookup.TryGetValue(rhs, out linkedListNode))
			{
				this.lookup.Remove(rhs);
				this.delegates.Remove(linkedListNode);
			}
		}

		public void Call(A a)
		{
			for (LinkedListNode<Action<A>> linkedListNode = this.delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value(a);
			}
		}

		private LinkedList<Action<A>> delegates = new LinkedList<Action<A>>();

		private Dictionary<Action<A>, LinkedListNode<Action<A>>> lookup = new Dictionary<Action<A>, LinkedListNode<Action<A>>>();
	}
}
