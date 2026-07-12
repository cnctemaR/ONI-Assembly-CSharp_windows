using System;
using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class CanonicalXmlNodeList : XmlNodeList, IList, ICollection, IEnumerable
	{
		internal CanonicalXmlNodeList()
		{
			this._nodeArray = new ArrayList();
		}

		public override XmlNode Item(int index)
		{
			return (XmlNode)this._nodeArray[index];
		}

		public override IEnumerator GetEnumerator()
		{
			return this._nodeArray.GetEnumerator();
		}

		public override int Count
		{
			get
			{
				return this._nodeArray.Count;
			}
		}

		public int Add(object value)
		{
			if (!(value is XmlNode))
			{
				throw new ArgumentException("Type of input object is invalid.", "node");
			}
			return this._nodeArray.Add(value);
		}

		public void Clear()
		{
			this._nodeArray.Clear();
		}

		public bool Contains(object value)
		{
			return this._nodeArray.Contains(value);
		}

		public int IndexOf(object value)
		{
			return this._nodeArray.IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is XmlNode))
			{
				throw new ArgumentException("Type of input object is invalid.", "value");
			}
			this._nodeArray.Insert(index, value);
		}

		public void Remove(object value)
		{
			this._nodeArray.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this._nodeArray.RemoveAt(index);
		}

		public bool IsFixedSize
		{
			get
			{
				return this._nodeArray.IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this._nodeArray.IsReadOnly;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this._nodeArray[index];
			}
			set
			{
				if (!(value is XmlNode))
				{
					throw new ArgumentException("Type of input object is invalid.", "value");
				}
				this._nodeArray[index] = value;
			}
		}

		public void CopyTo(Array array, int index)
		{
			this._nodeArray.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get
			{
				return this._nodeArray.SyncRoot;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this._nodeArray.IsSynchronized;
			}
		}

		private ArrayList _nodeArray;
	}
}
