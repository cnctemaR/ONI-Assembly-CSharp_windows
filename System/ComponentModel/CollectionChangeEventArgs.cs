using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class CollectionChangeEventArgs : EventArgs
	{
		public CollectionChangeEventArgs(CollectionChangeAction action, object element)
		{
			this.action = action;
			this.element = element;
		}

		public virtual CollectionChangeAction Action
		{
			get
			{
				return this.action;
			}
		}

		public virtual object Element
		{
			get
			{
				return this.element;
			}
		}

		private CollectionChangeAction action;

		private object element;
	}
}
