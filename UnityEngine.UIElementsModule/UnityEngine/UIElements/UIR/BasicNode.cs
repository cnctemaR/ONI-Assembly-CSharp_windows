using System;

namespace UnityEngine.UIElements.UIR
{
	internal class BasicNode<T> : LinkedPoolItem<BasicNode<T>>
	{
		public void InsertFirst(ref BasicNode<T> first)
		{
			bool flag = first == null;
			if (flag)
			{
				first = this;
			}
			else
			{
				this.next = first.next;
				first.next = this;
			}
		}

		public BasicNode<T> next;

		public T data;
	}
}
