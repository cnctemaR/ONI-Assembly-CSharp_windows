using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Schema
{
	public class XmlSchemaObjectTable
	{
		internal XmlSchemaObjectTable()
		{
		}

		internal void Add(XmlQualifiedName name, XmlSchemaObject value)
		{
			this.table.Add(name, value);
			this.entries.Add(new XmlSchemaObjectTable.XmlSchemaObjectEntry(name, value));
		}

		internal void Insert(XmlQualifiedName name, XmlSchemaObject value)
		{
			XmlSchemaObject xmlSchemaObject = null;
			if (this.table.TryGetValue(name, out xmlSchemaObject))
			{
				this.table[name] = value;
				int num = this.FindIndexByValue(xmlSchemaObject);
				this.entries[num] = new XmlSchemaObjectTable.XmlSchemaObjectEntry(name, value);
				return;
			}
			this.Add(name, value);
		}

		internal void Replace(XmlQualifiedName name, XmlSchemaObject value)
		{
			XmlSchemaObject xmlSchemaObject;
			if (this.table.TryGetValue(name, out xmlSchemaObject))
			{
				this.table[name] = value;
				int num = this.FindIndexByValue(xmlSchemaObject);
				this.entries[num] = new XmlSchemaObjectTable.XmlSchemaObjectEntry(name, value);
			}
		}

		internal void Clear()
		{
			this.table.Clear();
			this.entries.Clear();
		}

		internal void Remove(XmlQualifiedName name)
		{
			XmlSchemaObject xmlSchemaObject;
			if (this.table.TryGetValue(name, out xmlSchemaObject))
			{
				this.table.Remove(name);
				int num = this.FindIndexByValue(xmlSchemaObject);
				this.entries.RemoveAt(num);
			}
		}

		private int FindIndexByValue(XmlSchemaObject xso)
		{
			for (int i = 0; i < this.entries.Count; i++)
			{
				if (this.entries[i].xso == xso)
				{
					return i;
				}
			}
			return -1;
		}

		public int Count
		{
			get
			{
				return this.table.Count;
			}
		}

		public bool Contains(XmlQualifiedName name)
		{
			return this.table.ContainsKey(name);
		}

		public XmlSchemaObject this[XmlQualifiedName name]
		{
			get
			{
				XmlSchemaObject xmlSchemaObject;
				if (this.table.TryGetValue(name, out xmlSchemaObject))
				{
					return xmlSchemaObject;
				}
				return null;
			}
		}

		public ICollection Names
		{
			get
			{
				return new XmlSchemaObjectTable.NamesCollection(this.entries, this.table.Count);
			}
		}

		public ICollection Values
		{
			get
			{
				return new XmlSchemaObjectTable.ValuesCollection(this.entries, this.table.Count);
			}
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new XmlSchemaObjectTable.XSODictionaryEnumerator(this.entries, this.table.Count, XmlSchemaObjectTable.EnumeratorType.DictionaryEntry);
		}

		private Dictionary<XmlQualifiedName, XmlSchemaObject> table = new Dictionary<XmlQualifiedName, XmlSchemaObject>();

		private List<XmlSchemaObjectTable.XmlSchemaObjectEntry> entries = new List<XmlSchemaObjectTable.XmlSchemaObjectEntry>();

		internal enum EnumeratorType
		{
			Keys,
			Values,
			DictionaryEntry
		}

		internal struct XmlSchemaObjectEntry
		{
			public XmlSchemaObjectEntry(XmlQualifiedName name, XmlSchemaObject value)
			{
				this.qname = name;
				this.xso = value;
			}

			public XmlSchemaObject IsMatch(string localName, string ns)
			{
				if (localName == this.qname.Name && ns == this.qname.Namespace)
				{
					return this.xso;
				}
				return null;
			}

			public void Reset()
			{
				this.qname = null;
				this.xso = null;
			}

			internal XmlQualifiedName qname;

			internal XmlSchemaObject xso;
		}

		internal class NamesCollection : ICollection, IEnumerable
		{
			internal NamesCollection(List<XmlSchemaObjectTable.XmlSchemaObjectEntry> entries, int size)
			{
				this.entries = entries;
				this.size = size;
			}

			public int Count
			{
				get
				{
					return this.size;
				}
			}

			public object SyncRoot
			{
				get
				{
					return ((ICollection)this.entries).SyncRoot;
				}
			}

			public bool IsSynchronized
			{
				get
				{
					return ((ICollection)this.entries).IsSynchronized;
				}
			}

			public void CopyTo(Array array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex");
				}
				for (int i = 0; i < this.size; i++)
				{
					array.SetValue(this.entries[i].qname, arrayIndex++);
				}
			}

			public IEnumerator GetEnumerator()
			{
				return new XmlSchemaObjectTable.XSOEnumerator(this.entries, this.size, XmlSchemaObjectTable.EnumeratorType.Keys);
			}

			private List<XmlSchemaObjectTable.XmlSchemaObjectEntry> entries;

			private int size;
		}

		internal class ValuesCollection : ICollection, IEnumerable
		{
			internal ValuesCollection(List<XmlSchemaObjectTable.XmlSchemaObjectEntry> entries, int size)
			{
				this.entries = entries;
				this.size = size;
			}

			public int Count
			{
				get
				{
					return this.size;
				}
			}

			public object SyncRoot
			{
				get
				{
					return ((ICollection)this.entries).SyncRoot;
				}
			}

			public bool IsSynchronized
			{
				get
				{
					return ((ICollection)this.entries).IsSynchronized;
				}
			}

			public void CopyTo(Array array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex");
				}
				for (int i = 0; i < this.size; i++)
				{
					array.SetValue(this.entries[i].xso, arrayIndex++);
				}
			}

			public IEnumerator GetEnumerator()
			{
				return new XmlSchemaObjectTable.XSOEnumerator(this.entries, this.size, XmlSchemaObjectTable.EnumeratorType.Values);
			}

			private List<XmlSchemaObjectTable.XmlSchemaObjectEntry> entries;

			private int size;
		}

		internal class XSOEnumerator : IEnumerator
		{
			internal XSOEnumerator(List<XmlSchemaObjectTable.XmlSchemaObjectEntry> entries, int size, XmlSchemaObjectTable.EnumeratorType enumType)
			{
				this.entries = entries;
				this.size = size;
				this.enumType = enumType;
				this.currentIndex = -1;
			}

			public object Current
			{
				get
				{
					if (this.currentIndex == -1)
					{
						throw new InvalidOperationException(Res.GetString("Enumeration has not started. Call MoveNext.", new object[] { string.Empty }));
					}
					if (this.currentIndex >= this.size)
					{
						throw new InvalidOperationException(Res.GetString("Enumeration has already finished.", new object[] { string.Empty }));
					}
					switch (this.enumType)
					{
					case XmlSchemaObjectTable.EnumeratorType.Keys:
						return this.currentKey;
					case XmlSchemaObjectTable.EnumeratorType.Values:
						return this.currentValue;
					case XmlSchemaObjectTable.EnumeratorType.DictionaryEntry:
						return new DictionaryEntry(this.currentKey, this.currentValue);
					default:
						return null;
					}
				}
			}

			public bool MoveNext()
			{
				if (this.currentIndex >= this.size - 1)
				{
					this.currentValue = null;
					this.currentKey = null;
					return false;
				}
				this.currentIndex++;
				this.currentValue = this.entries[this.currentIndex].xso;
				this.currentKey = this.entries[this.currentIndex].qname;
				return true;
			}

			public void Reset()
			{
				this.currentIndex = -1;
				this.currentValue = null;
				this.currentKey = null;
			}

			private List<XmlSchemaObjectTable.XmlSchemaObjectEntry> entries;

			private XmlSchemaObjectTable.EnumeratorType enumType;

			protected int currentIndex;

			protected int size;

			protected XmlQualifiedName currentKey;

			protected XmlSchemaObject currentValue;
		}

		internal class XSODictionaryEnumerator : XmlSchemaObjectTable.XSOEnumerator, IDictionaryEnumerator, IEnumerator
		{
			internal XSODictionaryEnumerator(List<XmlSchemaObjectTable.XmlSchemaObjectEntry> entries, int size, XmlSchemaObjectTable.EnumeratorType enumType)
				: base(entries, size, enumType)
			{
			}

			public DictionaryEntry Entry
			{
				get
				{
					if (this.currentIndex == -1)
					{
						throw new InvalidOperationException(Res.GetString("Enumeration has not started. Call MoveNext.", new object[] { string.Empty }));
					}
					if (this.currentIndex >= this.size)
					{
						throw new InvalidOperationException(Res.GetString("Enumeration has already finished.", new object[] { string.Empty }));
					}
					return new DictionaryEntry(this.currentKey, this.currentValue);
				}
			}

			public object Key
			{
				get
				{
					if (this.currentIndex == -1)
					{
						throw new InvalidOperationException(Res.GetString("Enumeration has not started. Call MoveNext.", new object[] { string.Empty }));
					}
					if (this.currentIndex >= this.size)
					{
						throw new InvalidOperationException(Res.GetString("Enumeration has already finished.", new object[] { string.Empty }));
					}
					return this.currentKey;
				}
			}

			public object Value
			{
				get
				{
					if (this.currentIndex == -1)
					{
						throw new InvalidOperationException(Res.GetString("Enumeration has not started. Call MoveNext.", new object[] { string.Empty }));
					}
					if (this.currentIndex >= this.size)
					{
						throw new InvalidOperationException(Res.GetString("Enumeration has already finished.", new object[] { string.Empty }));
					}
					return this.currentValue;
				}
			}
		}
	}
}
