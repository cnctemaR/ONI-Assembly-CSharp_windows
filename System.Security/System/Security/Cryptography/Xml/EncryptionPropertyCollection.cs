using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography.Xml
{
	public sealed class EncryptionPropertyCollection : IList, ICollection, IEnumerable
	{
		public EncryptionPropertyCollection()
		{
			this._props = new ArrayList();
		}

		public IEnumerator GetEnumerator()
		{
			return this._props.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this._props.Count;
			}
		}

		int IList.Add(object value)
		{
			if (!(value is EncryptionProperty))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			return this._props.Add(value);
		}

		public int Add(EncryptionProperty value)
		{
			return this._props.Add(value);
		}

		public void Clear()
		{
			this._props.Clear();
		}

		bool IList.Contains(object value)
		{
			if (!(value is EncryptionProperty))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			return this._props.Contains(value);
		}

		public bool Contains(EncryptionProperty value)
		{
			return this._props.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			if (!(value is EncryptionProperty))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			return this._props.IndexOf(value);
		}

		public int IndexOf(EncryptionProperty value)
		{
			return this._props.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			if (!(value is EncryptionProperty))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			this._props.Insert(index, value);
		}

		public void Insert(int index, EncryptionProperty value)
		{
			this._props.Insert(index, value);
		}

		void IList.Remove(object value)
		{
			if (!(value is EncryptionProperty))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			this._props.Remove(value);
		}

		public void Remove(EncryptionProperty value)
		{
			this._props.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this._props.RemoveAt(index);
		}

		public bool IsFixedSize
		{
			get
			{
				return this._props.IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this._props.IsReadOnly;
			}
		}

		public EncryptionProperty Item(int index)
		{
			return (EncryptionProperty)this._props[index];
		}

		[IndexerName("ItemOf")]
		public EncryptionProperty this[int index]
		{
			get
			{
				return (EncryptionProperty)((IList)this)[index];
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
				return this._props[index];
			}
			set
			{
				if (!(value is EncryptionProperty))
				{
					throw new ArgumentException("Type of input object is invalid.", "value");
				}
				this._props[index] = value;
			}
		}

		public void CopyTo(Array array, int index)
		{
			this._props.CopyTo(array, index);
		}

		public void CopyTo(EncryptionProperty[] array, int index)
		{
			this._props.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get
			{
				return this._props.SyncRoot;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this._props.IsSynchronized;
			}
		}

		private ArrayList _props;
	}
}
