using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography.Xml
{
	public sealed class ReferenceList : IEnumerable, IList, ICollection
	{
		public ReferenceList()
		{
			this.list = new ArrayList();
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (EncryptedReference)value;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		[IndexerName("ItemOf")]
		public EncryptedReference this[int index]
		{
			get
			{
				return (EncryptedReference)this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		public int Add(object value)
		{
			if (!(value is EncryptedReference))
			{
				throw new ArgumentException("value");
			}
			return this.list.Add(value);
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public bool Contains(object value)
		{
			return this.list.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public EncryptedReference Item(int index)
		{
			return (EncryptedReference)this.list[index];
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is EncryptedReference))
			{
				throw new ArgumentException("value");
			}
			this.list.Insert(index, value);
		}

		public void Remove(object value)
		{
			this.list.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}

		private ArrayList list;
	}
}
