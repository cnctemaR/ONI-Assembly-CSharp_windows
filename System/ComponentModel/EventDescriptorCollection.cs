using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[ComVisible(true)]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true)]
	public class EventDescriptorCollection : ICollection, IEnumerable, IList
	{
		public EventDescriptorCollection(EventDescriptor[] events)
		{
			this.events = events;
			if (events == null)
			{
				this.events = new EventDescriptor[0];
				this.eventCount = 0;
			}
			else
			{
				this.eventCount = this.events.Length;
			}
			this.eventsOwned = true;
		}

		public EventDescriptorCollection(EventDescriptor[] events, bool readOnly)
			: this(events)
		{
			this.readOnly = readOnly;
		}

		private EventDescriptorCollection(EventDescriptor[] events, int eventCount, string[] namedSort, IComparer comparer)
		{
			this.eventsOwned = false;
			if (namedSort != null)
			{
				this.namedSort = (string[])namedSort.Clone();
			}
			this.comparer = comparer;
			this.events = events;
			this.eventCount = eventCount;
			this.needSort = true;
		}

		public int Count
		{
			get
			{
				return this.eventCount;
			}
		}

		public virtual EventDescriptor this[int index]
		{
			get
			{
				if (index >= this.eventCount)
				{
					throw new IndexOutOfRangeException();
				}
				this.EnsureEventsOwned();
				return this.events[index];
			}
		}

		public virtual EventDescriptor this[string name]
		{
			get
			{
				return this.Find(name, false);
			}
		}

		public int Add(EventDescriptor value)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			this.EnsureSize(this.eventCount + 1);
			EventDescriptor[] array = this.events;
			int num = this.eventCount;
			this.eventCount = num + 1;
			array[num] = value;
			return this.eventCount - 1;
		}

		public void Clear()
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			this.eventCount = 0;
		}

		public bool Contains(EventDescriptor value)
		{
			return this.IndexOf(value) >= 0;
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.EnsureEventsOwned();
			Array.Copy(this.events, 0, array, index, this.Count);
		}

		private void EnsureEventsOwned()
		{
			if (!this.eventsOwned)
			{
				this.eventsOwned = true;
				if (this.events != null)
				{
					EventDescriptor[] array = new EventDescriptor[this.Count];
					Array.Copy(this.events, 0, array, 0, this.Count);
					this.events = array;
				}
			}
			if (this.needSort)
			{
				this.needSort = false;
				this.InternalSort(this.namedSort);
			}
		}

		private void EnsureSize(int sizeNeeded)
		{
			if (sizeNeeded <= this.events.Length)
			{
				return;
			}
			if (this.events == null || this.events.Length == 0)
			{
				this.eventCount = 0;
				this.events = new EventDescriptor[sizeNeeded];
				return;
			}
			this.EnsureEventsOwned();
			EventDescriptor[] array = new EventDescriptor[Math.Max(sizeNeeded, this.events.Length * 2)];
			Array.Copy(this.events, 0, array, 0, this.eventCount);
			this.events = array;
		}

		public virtual EventDescriptor Find(string name, bool ignoreCase)
		{
			EventDescriptor eventDescriptor = null;
			if (ignoreCase)
			{
				for (int i = 0; i < this.Count; i++)
				{
					if (string.Equals(this.events[i].Name, name, StringComparison.OrdinalIgnoreCase))
					{
						eventDescriptor = this.events[i];
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < this.Count; j++)
				{
					if (string.Equals(this.events[j].Name, name, StringComparison.Ordinal))
					{
						eventDescriptor = this.events[j];
						break;
					}
				}
			}
			return eventDescriptor;
		}

		public int IndexOf(EventDescriptor value)
		{
			return Array.IndexOf<EventDescriptor>(this.events, value, 0, this.eventCount);
		}

		public void Insert(int index, EventDescriptor value)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			this.EnsureSize(this.eventCount + 1);
			if (index < this.eventCount)
			{
				Array.Copy(this.events, index, this.events, index + 1, this.eventCount - index);
			}
			this.events[index] = value;
			this.eventCount++;
		}

		public void Remove(EventDescriptor value)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			int num = this.IndexOf(value);
			if (num != -1)
			{
				this.RemoveAt(num);
			}
		}

		public void RemoveAt(int index)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			if (index < this.eventCount - 1)
			{
				Array.Copy(this.events, index + 1, this.events, index, this.eventCount - index - 1);
			}
			this.events[this.eventCount - 1] = null;
			this.eventCount--;
		}

		public IEnumerator GetEnumerator()
		{
			if (this.events.Length == this.eventCount)
			{
				return this.events.GetEnumerator();
			}
			return new ArraySubsetEnumerator(this.events, this.eventCount);
		}

		public virtual EventDescriptorCollection Sort()
		{
			return new EventDescriptorCollection(this.events, this.eventCount, this.namedSort, this.comparer);
		}

		public virtual EventDescriptorCollection Sort(string[] names)
		{
			return new EventDescriptorCollection(this.events, this.eventCount, names, this.comparer);
		}

		public virtual EventDescriptorCollection Sort(string[] names, IComparer comparer)
		{
			return new EventDescriptorCollection(this.events, this.eventCount, names, comparer);
		}

		public virtual EventDescriptorCollection Sort(IComparer comparer)
		{
			return new EventDescriptorCollection(this.events, this.eventCount, this.namedSort, comparer);
		}

		protected void InternalSort(string[] names)
		{
			if (this.events == null || this.events.Length == 0)
			{
				return;
			}
			this.InternalSort(this.comparer);
			if (names != null && names.Length != 0)
			{
				ArrayList arrayList = new ArrayList(this.events);
				int num = 0;
				int num2 = this.events.Length;
				for (int i = 0; i < names.Length; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						EventDescriptor eventDescriptor = (EventDescriptor)arrayList[j];
						if (eventDescriptor != null && eventDescriptor.Name.Equals(names[i]))
						{
							this.events[num++] = eventDescriptor;
							arrayList[j] = null;
							break;
						}
					}
				}
				for (int k = 0; k < num2; k++)
				{
					if (arrayList[k] != null)
					{
						this.events[num++] = (EventDescriptor)arrayList[k];
					}
				}
			}
		}

		protected void InternalSort(IComparer sorter)
		{
			if (sorter == null)
			{
				TypeDescriptor.SortDescriptorArray(this);
				return;
			}
			Array.Sort(this.events, sorter);
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
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
				return null;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (this.readOnly)
				{
					throw new NotSupportedException();
				}
				if (index >= this.eventCount)
				{
					throw new IndexOutOfRangeException();
				}
				this.EnsureEventsOwned();
				this.events[index] = (EventDescriptor)value;
			}
		}

		int IList.Add(object value)
		{
			return this.Add((EventDescriptor)value);
		}

		void IList.Clear()
		{
			this.Clear();
		}

		bool IList.Contains(object value)
		{
			return this.Contains((EventDescriptor)value);
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf((EventDescriptor)value);
		}

		void IList.Insert(int index, object value)
		{
			this.Insert(index, (EventDescriptor)value);
		}

		void IList.Remove(object value)
		{
			this.Remove((EventDescriptor)value);
		}

		void IList.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this.readOnly;
			}
		}

		private EventDescriptor[] events;

		private string[] namedSort;

		private IComparer comparer;

		private bool eventsOwned = true;

		private bool needSort;

		private int eventCount;

		private bool readOnly;

		public static readonly EventDescriptorCollection Empty = new EventDescriptorCollection(null, true);
	}
}
