using System;

namespace System.Collections.Specialized
{
	[Serializable]
	public class StringCollection : IList, ICollection, IEnumerable
	{
		public string this[int index]
		{
			get
			{
				return (string)this.data[index];
			}
			set
			{
				this.data[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return this.data.Count;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public int Add(string value)
		{
			return this.data.Add(value);
		}

		public void AddRange(string[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.data.AddRange(value);
		}

		public void Clear()
		{
			this.data.Clear();
		}

		public bool Contains(string value)
		{
			return this.data.Contains(value);
		}

		public void CopyTo(string[] array, int index)
		{
			this.data.CopyTo(array, index);
		}

		public StringEnumerator GetEnumerator()
		{
			return new StringEnumerator(this);
		}

		public int IndexOf(string value)
		{
			return this.data.IndexOf(value);
		}

		public void Insert(int index, string value)
		{
			this.data.Insert(index, value);
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public void Remove(string value)
		{
			this.data.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this.data.RemoveAt(index);
		}

		public object SyncRoot
		{
			get
			{
				return this.data.SyncRoot;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (string)value;
			}
		}

		int IList.Add(object value)
		{
			return this.Add((string)value);
		}

		bool IList.Contains(object value)
		{
			return this.Contains((string)value);
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf((string)value);
		}

		void IList.Insert(int index, object value)
		{
			this.Insert(index, (string)value);
		}

		void IList.Remove(object value)
		{
			this.Remove((string)value);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.data.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.data.GetEnumerator();
		}

		private ArrayList data = new ArrayList();
	}
}
