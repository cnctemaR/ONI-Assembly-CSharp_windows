using System;

namespace System.ComponentModel
{
	public class CollectionChangeEventArgs : EventArgs
	{
		public CollectionChangeEventArgs(CollectionChangeAction action, object element)
		{
			this.changeAction = action;
			this.theElement = element;
		}

		public virtual CollectionChangeAction Action
		{
			get
			{
				return this.changeAction;
			}
		}

		public virtual object Element
		{
			get
			{
				return this.theElement;
			}
		}

		private CollectionChangeAction changeAction;

		private object theElement;
	}
}
