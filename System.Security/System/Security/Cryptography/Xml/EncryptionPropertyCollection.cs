using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography.Xml
{
	public sealed class EncryptionPropertyCollection : IEnumerable, IList, ICollection
	{
		public EncryptionPropertyCollection()
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
				this[index] = (EncryptionProperty)value;
			}
		}

		bool IList.Contains(object value)
		{
			return this.Contains((EncryptionProperty)value);
		}

		int IList.Add(object value)
		{
			return this.Add((EncryptionProperty)value);
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf((EncryptionProperty)value);
		}

		void IList.Insert(int index, object value)
		{
			this.Insert(index, (EncryptionProperty)value);
		}

		void IList.Remove(object value)
		{
			this.Remove((EncryptionProperty)value);
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return this.list.IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.list.IsReadOnly;
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
		public EncryptionProperty this[int index]
		{
			get
			{
				return (EncryptionProperty)this.list[index];
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

		public int Add(EncryptionProperty value)
		{
			return this.list.Add(value);
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public bool Contains(EncryptionProperty value)
		{
			return this.list.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public void CopyTo(EncryptionProperty[] array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public int IndexOf(EncryptionProperty value)
		{
			return this.list.IndexOf(value);
		}

		public void Insert(int index, EncryptionProperty value)
		{
			this.list.Insert(index, value);
		}

		public EncryptionProperty Item(int index)
		{
			return (EncryptionProperty)this.list[index];
		}

		public void Remove(EncryptionProperty value)
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
