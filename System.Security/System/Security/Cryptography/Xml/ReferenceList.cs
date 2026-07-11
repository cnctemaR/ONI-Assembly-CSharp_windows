using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography.Xml
{
	public sealed class ReferenceList : IList, ICollection, IEnumerable
	{
		public ReferenceList()
		{
			this._references = new ArrayList();
		}

		public IEnumerator GetEnumerator()
		{
			return this._references.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this._references.Count;
			}
		}

		public int Add(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!(value is DataReference) && !(value is KeyReference))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			return this._references.Add(value);
		}

		public void Clear()
		{
			this._references.Clear();
		}

		public bool Contains(object value)
		{
			return this._references.Contains(value);
		}

		public int IndexOf(object value)
		{
			return this._references.IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!(value is DataReference) && !(value is KeyReference))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			this._references.Insert(index, value);
		}

		public void Remove(object value)
		{
			this._references.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this._references.RemoveAt(index);
		}

		public EncryptedReference Item(int index)
		{
			return (EncryptedReference)this._references[index];
		}

		[IndexerName("ItemOf")]
		public EncryptedReference this[int index]
		{
			get
			{
				return this.Item(index);
			}
			set
			{
				((IList)this)[index] = value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this._references[index];
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (!(value is DataReference) && !(value is KeyReference))
				{
					throw new ArgumentException("Type of input object is invalid.", "value");
				}
				this._references[index] = value;
			}
		}

		public void CopyTo(Array array, int index)
		{
			this._references.CopyTo(array, index);
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this._references.IsFixedSize;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this._references.IsReadOnly;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this._references.SyncRoot;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this._references.IsSynchronized;
			}
		}

		private ArrayList _references;
	}
}
