using System;
using System.Collections;

namespace System.Diagnostics
{
	public class TraceListenerCollection : IList, ICollection, IEnumerable
	{
		internal TraceListenerCollection()
			: this(true)
		{
		}

		internal TraceListenerCollection(bool addDefault)
		{
			if (addDefault)
			{
				this.Add(new DefaultTraceListener());
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.listeners[index];
			}
			set
			{
				TraceListener traceListener = (TraceListener)value;
				this.InitializeListener(traceListener);
				this[index] = traceListener;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.listeners.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.listeners.SyncRoot;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this.listeners.IsFixedSize;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.listeners.IsReadOnly;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.listeners.CopyTo(array, index);
		}

		int IList.Add(object value)
		{
			if (value is TraceListener)
			{
				return this.Add((TraceListener)value);
			}
			throw new NotSupportedException(global::Locale.GetText("You can only add TraceListener objects to the collection"));
		}

		bool IList.Contains(object value)
		{
			return value is TraceListener && this.listeners.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			if (value is TraceListener)
			{
				return this.listeners.IndexOf(value);
			}
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			if (value is TraceListener)
			{
				this.Insert(index, (TraceListener)value);
				return;
			}
			throw new NotSupportedException(global::Locale.GetText("You can only insert TraceListener objects into the collection"));
		}

		void IList.Remove(object value)
		{
			if (value is TraceListener)
			{
				this.listeners.Remove(value);
			}
		}

		public int Count
		{
			get
			{
				return this.listeners.Count;
			}
		}

		public TraceListener this[string name]
		{
			get
			{
				object syncRoot = this.listeners.SyncRoot;
				lock (syncRoot)
				{
					foreach (object obj in this.listeners)
					{
						TraceListener traceListener = (TraceListener)obj;
						if (traceListener.Name == name)
						{
							return traceListener;
						}
					}
				}
				return null;
			}
		}

		public TraceListener this[int index]
		{
			get
			{
				return (TraceListener)this.listeners[index];
			}
			set
			{
				this.InitializeListener(value);
				this.listeners[index] = value;
			}
		}

		public int Add(TraceListener listener)
		{
			this.InitializeListener(listener);
			return this.listeners.Add(listener);
		}

		internal void Add(TraceListener listener, TraceImplSettings settings)
		{
			listener.IndentLevel = settings.IndentLevel;
			listener.IndentSize = settings.IndentSize;
			this.listeners.Add(listener);
		}

		private void InitializeListener(TraceListener listener)
		{
			listener.IndentLevel = TraceImpl.IndentLevel;
			listener.IndentSize = TraceImpl.IndentSize;
		}

		private void InitializeRange(IList listeners)
		{
			int count = listeners.Count;
			for (int num = 0; num != count; num++)
			{
				this.InitializeListener((TraceListener)listeners[num]);
			}
		}

		public void AddRange(TraceListener[] value)
		{
			this.InitializeRange(value);
			this.listeners.AddRange(value);
		}

		public void AddRange(TraceListenerCollection value)
		{
			this.InitializeRange(value);
			this.listeners.AddRange(value.listeners);
		}

		public void Clear()
		{
			this.listeners.Clear();
		}

		public bool Contains(TraceListener listener)
		{
			return this.listeners.Contains(listener);
		}

		public void CopyTo(TraceListener[] listeners, int index)
		{
			listeners.CopyTo(listeners, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.listeners.GetEnumerator();
		}

		public int IndexOf(TraceListener listener)
		{
			return this.listeners.IndexOf(listener);
		}

		public void Insert(int index, TraceListener listener)
		{
			this.InitializeListener(listener);
			this.listeners.Insert(index, listener);
		}

		public void Remove(string name)
		{
			TraceListener traceListener = null;
			object syncRoot = this.listeners.SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in this.listeners)
				{
					TraceListener traceListener2 = (TraceListener)obj;
					if (traceListener2.Name == name)
					{
						traceListener = traceListener2;
						break;
					}
				}
				if (traceListener == null)
				{
					throw new ArgumentException(global::Locale.GetText("TraceListener " + name + " was not in the collection"));
				}
				this.listeners.Remove(traceListener);
			}
		}

		public void Remove(TraceListener listener)
		{
			this.listeners.Remove(listener);
		}

		public void RemoveAt(int index)
		{
			this.listeners.RemoveAt(index);
		}

		private ArrayList listeners = ArrayList.Synchronized(new ArrayList(1));
	}
}
