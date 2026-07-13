using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text
{
	public class FastAction<A, B>
	{
		public void Add(Action<A, B> rhs)
		{
			bool flag = this.lookup.ContainsKey(rhs);
			if (!flag)
			{
				this.lookup[rhs] = this.delegates.AddLast(rhs);
			}
		}

		public void Remove(Action<A, B> rhs)
		{
			LinkedListNode<Action<A, B>> linkedListNode;
			bool flag = this.lookup.TryGetValue(rhs, out linkedListNode);
			if (flag)
			{
				this.lookup.Remove(rhs);
				this.delegates.Remove(linkedListNode);
			}
		}

		public void Call(A a, B b)
		{
			for (LinkedListNode<Action<A, B>> linkedListNode = this.delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value(a, b);
			}
		}

		private LinkedList<Action<A, B>> delegates = new LinkedList<Action<A, B>>();

		private Dictionary<Action<A, B>, LinkedListNode<Action<A, B>>> lookup = new Dictionary<Action<A, B>, LinkedListNode<Action<A, B>>>();
	}
}
