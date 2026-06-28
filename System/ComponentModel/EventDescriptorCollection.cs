using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public class EventDescriptorCollection : IList, ICollection, IEnumerable
	{
		private EventDescriptorCollection()
		{
		}

		internal EventDescriptorCollection(ArrayList list)
		{
			this.eventList = list;
		}

		public EventDescriptorCollection(EventDescriptor[] events)
			: this(events, false)
		{
		}

		public EventDescriptorCollection(EventDescriptor[] events, bool readOnly)
		{
			this.isReadOnly = readOnly;
			if (events == null)
			{
				return;
			}
			for (int i = 0; i < events.Length; i++)
			{
				this.Add(events[i]);
			}
		}

		void IList.Clear()
		{
			this.Clear();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void IList.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		int IList.Add(object value)
		{
			return this.Add((EventDescriptor)value);
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

		bool IList.IsFixedSize
		{
			get
			{
				return this.isReadOnly;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.eventList[index];
			}
			set
			{
				if (this.isReadOnly)
				{
					throw new NotSupportedException("The collection is read-only");
				}
				this.eventList[index] = value;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.eventList.CopyTo(array, index);
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

		public int Add(EventDescriptor value)
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException("The collection is read-only");
			}
			return this.eventList.Add(value);
		}

		public void Clear()
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException("The collection is read-only");
			}
			this.eventList.Clear();
		}

		public bool Contains(EventDescriptor value)
		{
			return this.eventList.Contains(value);
		}

		public virtual EventDescriptor Find(string name, bool ignoreCase)
		{
			foreach (object obj in this.eventList)
			{
				EventDescriptor eventDescriptor = (EventDescriptor)obj;
				if (ignoreCase)
				{
					if (string.Compare(name, eventDescriptor.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return eventDescriptor;
					}
				}
				else if (string.Compare(name, eventDescriptor.Name, StringComparison.Ordinal) == 0)
				{
					return eventDescriptor;
				}
			}
			return null;
		}

		public IEnumerator GetEnumerator()
		{
			return this.eventList.GetEnumerator();
		}

		public int IndexOf(EventDescriptor value)
		{
			return this.eventList.IndexOf(value);
		}

		public void Insert(int index, EventDescriptor value)
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException("The collection is read-only");
			}
			this.eventList.Insert(index, value);
		}

		public void Remove(EventDescriptor value)
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException("The collection is read-only");
			}
			this.eventList.Remove(value);
		}

		public void RemoveAt(int index)
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException("The collection is read-only");
			}
			this.eventList.RemoveAt(index);
		}

		public virtual EventDescriptorCollection Sort()
		{
			EventDescriptorCollection eventDescriptorCollection = this.CloneCollection();
			eventDescriptorCollection.InternalSort(null);
			return eventDescriptorCollection;
		}

		public virtual EventDescriptorCollection Sort(IComparer comparer)
		{
			EventDescriptorCollection eventDescriptorCollection = this.CloneCollection();
			eventDescriptorCollection.InternalSort(comparer);
			return eventDescriptorCollection;
		}

		public virtual EventDescriptorCollection Sort(string[] order)
		{
			EventDescriptorCollection eventDescriptorCollection = this.CloneCollection();
			eventDescriptorCollection.InternalSort(order);
			return eventDescriptorCollection;
		}

		public virtual EventDescriptorCollection Sort(string[] order, IComparer comparer)
		{
			EventDescriptorCollection eventDescriptorCollection = this.CloneCollection();
			if (order != null)
			{
				ArrayList arrayList = eventDescriptorCollection.ExtractItems(order);
				eventDescriptorCollection.InternalSort(comparer);
				arrayList.AddRange(eventDescriptorCollection.eventList);
				eventDescriptorCollection.eventList = arrayList;
			}
			else
			{
				eventDescriptorCollection.InternalSort(comparer);
			}
			return eventDescriptorCollection;
		}

		protected void InternalSort(IComparer comparer)
		{
			if (comparer == null)
			{
				comparer = MemberDescriptor.DefaultComparer;
			}
			this.eventList.Sort(comparer);
		}

		protected void InternalSort(string[] order)
		{
			if (order != null)
			{
				ArrayList arrayList = this.ExtractItems(order);
				this.InternalSort(null);
				arrayList.AddRange(this.eventList);
				this.eventList = arrayList;
			}
			else
			{
				this.InternalSort(null);
			}
		}

		private ArrayList ExtractItems(string[] names)
		{
			ArrayList arrayList = new ArrayList(this.eventList.Count);
			object[] array = new object[names.Length];
			for (int i = 0; i < this.eventList.Count; i++)
			{
				EventDescriptor eventDescriptor = (EventDescriptor)this.eventList[i];
				int num = Array.IndexOf<string>(names, eventDescriptor.Name);
				if (num != -1)
				{
					array[num] = eventDescriptor;
					this.eventList.RemoveAt(i);
					i--;
				}
			}
			foreach (object obj in array)
			{
				if (obj != null)
				{
					arrayList.Add(obj);
				}
			}
			return arrayList;
		}

		private EventDescriptorCollection CloneCollection()
		{
			return new EventDescriptorCollection
			{
				eventList = (ArrayList)this.eventList.Clone()
			};
		}

		internal EventDescriptorCollection Filter(Attribute[] attributes)
		{
			EventDescriptorCollection eventDescriptorCollection = new EventDescriptorCollection();
			foreach (object obj in this.eventList)
			{
				EventDescriptor eventDescriptor = (EventDescriptor)obj;
				if (eventDescriptor.Attributes.Contains(attributes))
				{
					eventDescriptorCollection.eventList.Add(eventDescriptor);
				}
			}
			return eventDescriptorCollection;
		}

		public int Count
		{
			get
			{
				return this.eventList.Count;
			}
		}

		public virtual EventDescriptor this[string name]
		{
			get
			{
				return this.Find(name, false);
			}
		}

		public virtual EventDescriptor this[int index]
		{
			get
			{
				return (EventDescriptor)this.eventList[index];
			}
		}

		private ArrayList eventList = new ArrayList();

		private bool isReadOnly;

		public static readonly EventDescriptorCollection Empty = new EventDescriptorCollection(null, true);
	}
}
