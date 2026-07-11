using System;

namespace System.ComponentModel
{
	public sealed class EventHandlerList : IDisposable
	{
		public Delegate this[object key]
		{
			get
			{
				if (key == null)
				{
					return this.null_entry;
				}
				ListEntry listEntry = this.FindEntry(key);
				if (listEntry != null)
				{
					return listEntry.value;
				}
				return null;
			}
			set
			{
				this.AddHandler(key, value);
			}
		}

		public void AddHandler(object key, Delegate value)
		{
			if (key == null)
			{
				this.null_entry = Delegate.Combine(this.null_entry, value);
				return;
			}
			ListEntry listEntry = this.FindEntry(key);
			if (listEntry == null)
			{
				listEntry = new ListEntry();
				listEntry.key = key;
				listEntry.value = null;
				listEntry.next = this.entries;
				this.entries = listEntry;
			}
			listEntry.value = Delegate.Combine(listEntry.value, value);
		}

		public void AddHandlers(EventHandlerList listToAddFrom)
		{
			if (listToAddFrom == null)
			{
				return;
			}
			for (ListEntry next = listToAddFrom.entries; next != null; next = next.next)
			{
				this.AddHandler(next.key, next.value);
			}
		}

		public void RemoveHandler(object key, Delegate value)
		{
			if (key == null)
			{
				this.null_entry = Delegate.Remove(this.null_entry, value);
				return;
			}
			ListEntry listEntry = this.FindEntry(key);
			if (listEntry == null)
			{
				return;
			}
			listEntry.value = Delegate.Remove(listEntry.value, value);
		}

		public void Dispose()
		{
			this.entries = null;
		}

		private ListEntry FindEntry(object key)
		{
			for (ListEntry next = this.entries; next != null; next = next.next)
			{
				if (next.key == key)
				{
					return next;
				}
			}
			return null;
		}

		private ListEntry entries;

		private Delegate null_entry;
	}
}
