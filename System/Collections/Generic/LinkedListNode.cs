using System;
using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
	[ComVisible(false)]
	public sealed class LinkedListNode<T>
	{
		public LinkedListNode(T value)
		{
			this.item = value;
		}

		internal LinkedListNode(LinkedList<T> list, T value)
		{
			this.container = list;
			this.item = value;
			this.forward = this;
			this.back = this;
		}

		internal LinkedListNode(LinkedList<T> list, T value, LinkedListNode<T> previousNode, LinkedListNode<T> nextNode)
		{
			this.container = list;
			this.item = value;
			this.back = previousNode;
			this.forward = nextNode;
			previousNode.forward = this;
			nextNode.back = this;
		}

		internal void Detach()
		{
			this.back.forward = this.forward;
			this.forward.back = this.back;
			this.forward = (this.back = null);
			this.container = null;
		}

		internal void SelfReference(LinkedList<T> list)
		{
			this.forward = this;
			this.back = this;
			this.container = list;
		}

		internal void InsertBetween(LinkedListNode<T> previousNode, LinkedListNode<T> nextNode, LinkedList<T> list)
		{
			previousNode.forward = this;
			nextNode.back = this;
			this.forward = nextNode;
			this.back = previousNode;
			this.container = list;
		}

		public LinkedList<T> List
		{
			get
			{
				return this.container;
			}
		}

		public LinkedListNode<T> Next
		{
			get
			{
				return (this.container == null || this.forward == this.container.first) ? null : this.forward;
			}
		}

		public LinkedListNode<T> Previous
		{
			get
			{
				return (this.container == null || this == this.container.first) ? null : this.back;
			}
		}

		public T Value
		{
			get
			{
				return this.item;
			}
			set
			{
				this.item = value;
			}
		}

		private T item;

		private LinkedList<T> container;

		internal LinkedListNode<T> forward;

		internal LinkedListNode<T> back;
	}
}
