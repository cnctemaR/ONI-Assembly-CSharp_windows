using System;
using System.Collections.Generic;

namespace TMPro
{
	public class FastAction
	{
		public void Add(global::System.Action rhs)
		{
			if (this.lookup.ContainsKey(rhs))
			{
				return;
			}
			this.lookup[rhs] = this.delegates.AddLast(rhs);
		}

		public void Remove(global::System.Action rhs)
		{
			LinkedListNode<global::System.Action> linkedListNode;
			if (this.lookup.TryGetValue(rhs, out linkedListNode))
			{
				this.lookup.Remove(rhs);
				this.delegates.Remove(linkedListNode);
			}
		}

		public void Call()
		{
			for (LinkedListNode<global::System.Action> linkedListNode = this.delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value();
			}
		}

		private LinkedList<global::System.Action> delegates = new LinkedList<global::System.Action>();

		private Dictionary<global::System.Action, LinkedListNode<global::System.Action>> lookup = new Dictionary<global::System.Action, LinkedListNode<global::System.Action>>();
	}
}
