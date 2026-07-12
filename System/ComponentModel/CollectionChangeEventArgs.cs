using System;

namespace System.ComponentModel
{
	public class CollectionChangeEventArgs : EventArgs
	{
		public CollectionChangeEventArgs(CollectionChangeAction action, object element)
		{
			this.Action = action;
			this.Element = element;
		}

		public virtual CollectionChangeAction Action { get; }

		public virtual object Element { get; }
	}
}
