using System;
using System.Collections.Generic;

namespace TMPro
{
	public class FastAction
	{
		public void Add(Action rhs)
		{
			if (this.lookup.ContainsKey(rhs))
			{
				return;
			}
			this.lookup[rhs] = this.delegates.AddLast(rhs);
		}

		public void Remove(Action rhs)
		{
			LinkedListNode<Action> linkedListNode;
			if (this.lookup.TryGetValue(rhs, out linkedListNode))
			{
				this.lookup.Remove(rhs);
				this.delegates.Remove(linkedListNode);
			}
		}

		public void Call()
		{
			for (LinkedListNode<Action> linkedListNode = this.delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value();
			}
		}

		private LinkedList<Action> delegates = new LinkedList<Action>();

		private Dictionary<Action, LinkedListNode<Action>> lookup = new Dictionary<Action, LinkedListNode<Action>>();
	}
}
