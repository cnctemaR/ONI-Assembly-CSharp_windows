using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public sealed class EventHandlerList : IDisposable
	{
		public EventHandlerList()
		{
		}

		internal EventHandlerList(Component parent)
		{
			this.parent = parent;
		}

		public Delegate this[object key]
		{
			get
			{
				EventHandlerList.ListEntry listEntry = null;
				if (this.parent == null || this.parent.CanRaiseEventsInternal)
				{
					listEntry = this.Find(key);
				}
				if (listEntry != null)
				{
					return listEntry.handler;
				}
				return null;
			}
			set
			{
				EventHandlerList.ListEntry listEntry = this.Find(key);
				if (listEntry != null)
				{
					listEntry.handler = value;
					return;
				}
				this.head = new EventHandlerList.ListEntry(key, value, this.head);
			}
		}

		public void AddHandler(object key, Delegate value)
		{
			EventHandlerList.ListEntry listEntry = this.Find(key);
			if (listEntry != null)
			{
				listEntry.handler = Delegate.Combine(listEntry.handler, value);
				return;
			}
			this.head = new EventHandlerList.ListEntry(key, value, this.head);
		}

		public void AddHandlers(EventHandlerList listToAddFrom)
		{
			for (EventHandlerList.ListEntry next = listToAddFrom.head; next != null; next = next.next)
			{
				this.AddHandler(next.key, next.handler);
			}
		}

		public void Dispose()
		{
			this.head = null;
		}

		private EventHandlerList.ListEntry Find(object key)
		{
			EventHandlerList.ListEntry next = this.head;
			while (next != null && next.key != key)
			{
				next = next.next;
			}
			return next;
		}

		public void RemoveHandler(object key, Delegate value)
		{
			EventHandlerList.ListEntry listEntry = this.Find(key);
			if (listEntry != null)
			{
				listEntry.handler = Delegate.Remove(listEntry.handler, value);
			}
		}

		private EventHandlerList.ListEntry head;

		private Component parent;

		private sealed class ListEntry
		{
			public ListEntry(object key, Delegate handler, EventHandlerList.ListEntry next)
			{
				this.next = next;
				this.key = key;
				this.handler = handler;
			}

			internal EventHandlerList.ListEntry next;

			internal object key;

			internal Delegate handler;
		}
	}
}
