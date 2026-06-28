using System;
using System.Collections;

namespace System.Security.Cryptography
{
	public sealed class CryptographicAttributeObjectCollection : IEnumerable, ICollection
	{
		public CryptographicAttributeObjectCollection()
		{
			this._list = new ArrayList();
		}

		public CryptographicAttributeObjectCollection(CryptographicAttributeObject attribute)
			: this()
		{
			this._list.Add(attribute);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this._list.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new CryptographicAttributeObjectEnumerator(this._list);
		}

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this._list.IsSynchronized;
			}
		}

		public CryptographicAttributeObject this[int index]
		{
			get
			{
				return (CryptographicAttributeObject)this._list[index];
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public int Add(AsnEncodedData asnEncodedData)
		{
			if (asnEncodedData == null)
			{
				throw new ArgumentNullException("asnEncodedData");
			}
			AsnEncodedDataCollection asnEncodedDataCollection = new AsnEncodedDataCollection(asnEncodedData);
			return this.Add(new CryptographicAttributeObject(asnEncodedData.Oid, asnEncodedDataCollection));
		}

		public int Add(CryptographicAttributeObject attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			int num = -1;
			string value = attribute.Oid.Value;
			for (int i = 0; i < this._list.Count; i++)
			{
				if ((this._list[i] as CryptographicAttributeObject).Oid.Value == value)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				CryptographicAttributeObject cryptographicAttributeObject = this[num];
				foreach (AsnEncodedData asnEncodedData in attribute.Values)
				{
					cryptographicAttributeObject.Values.Add(asnEncodedData);
				}
				return num;
			}
			return this._list.Add(attribute);
		}

		public void CopyTo(CryptographicAttributeObject[] array, int index)
		{
			this._list.CopyTo(array, index);
		}

		public CryptographicAttributeObjectEnumerator GetEnumerator()
		{
			return new CryptographicAttributeObjectEnumerator(this._list);
		}

		public void Remove(CryptographicAttributeObject attribute)
		{
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute");
			}
			this._list.Remove(attribute);
		}

		private ArrayList _list;
	}
}
