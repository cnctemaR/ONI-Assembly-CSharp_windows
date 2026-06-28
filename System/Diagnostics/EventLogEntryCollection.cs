using System;
using System.Collections;

namespace System.Diagnostics
{
	public class EventLogEntryCollection : ICollection, IEnumerable
	{
		internal EventLogEntryCollection(EventLogImpl impl)
		{
			this._impl = impl;
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			EventLogEntry[] entries = this._impl.GetEntries();
			Array.Copy(entries, 0, array, index, entries.Length);
		}

		public int Count
		{
			get
			{
				return this._impl.EntryCount;
			}
		}

		public virtual EventLogEntry this[int index]
		{
			get
			{
				return this._impl[index];
			}
		}

		public void CopyTo(EventLogEntry[] eventLogEntries, int index)
		{
			EventLogEntry[] entries = this._impl.GetEntries();
			Array.Copy(entries, 0, eventLogEntries, index, entries.Length);
		}

		public IEnumerator GetEnumerator()
		{
			return new EventLogEntryCollection.EventLogEntryEnumerator(this._impl);
		}

		private readonly EventLogImpl _impl;

		private class EventLogEntryEnumerator : IEnumerator
		{
			internal EventLogEntryEnumerator(EventLogImpl impl)
			{
				this._impl = impl;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public EventLogEntry Current
			{
				get
				{
					if (this._currentEntry != null)
					{
						return this._currentEntry;
					}
					throw new InvalidOperationException("No current EventLog entry available, cursor is located before the first or after the last element of the enumeration.");
				}
			}

			public bool MoveNext()
			{
				this._currentIndex++;
				if (this._currentIndex >= this._impl.EntryCount)
				{
					this._currentEntry = null;
					return false;
				}
				this._currentEntry = this._impl[this._currentIndex];
				return true;
			}

			public void Reset()
			{
				this._currentIndex = -1;
			}

			private readonly EventLogImpl _impl;

			private int _currentIndex = -1;

			private EventLogEntry _currentEntry;
		}
	}
}
