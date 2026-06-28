using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.Xml.Schema
{
	public class XmlSchemaObjectTable
	{
		internal XmlSchemaObjectTable()
		{
			this.table = new HybridDictionary();
		}

		public int Count
		{
			get
			{
				return this.table.Count;
			}
		}

		public XmlSchemaObject this[XmlQualifiedName name]
		{
			get
			{
				return (XmlSchemaObject)this.table[name];
			}
		}

		public ICollection Names
		{
			get
			{
				return this.table.Keys;
			}
		}

		public ICollection Values
		{
			get
			{
				return this.table.Values;
			}
		}

		public bool Contains(XmlQualifiedName name)
		{
			return this.table.Contains(name);
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new XmlSchemaObjectTable.XmlSchemaObjectTableEnumerator(this);
		}

		internal void Add(XmlQualifiedName name, XmlSchemaObject value)
		{
			this.table[name] = value;
		}

		internal void Clear()
		{
			this.table.Clear();
		}

		internal void Set(XmlQualifiedName name, XmlSchemaObject value)
		{
			this.table[name] = value;
		}

		private HybridDictionary table;

		internal class XmlSchemaObjectTableEnumerator : IEnumerator, IDictionaryEnumerator
		{
			internal XmlSchemaObjectTableEnumerator(XmlSchemaObjectTable table)
			{
				this.tmp = table.table;
				this.xenum = (IDictionaryEnumerator)this.tmp.GetEnumerator();
			}

			bool IEnumerator.MoveNext()
			{
				return this.xenum.MoveNext();
			}

			void IEnumerator.Reset()
			{
				this.xenum.Reset();
			}

			object IEnumerator.Current
			{
				get
				{
					return this.xenum.Entry;
				}
			}

			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					return this.xenum.Entry;
				}
			}

			object IDictionaryEnumerator.Key
			{
				get
				{
					return (XmlQualifiedName)this.xenum.Key;
				}
			}

			object IDictionaryEnumerator.Value
			{
				get
				{
					return (XmlSchemaObject)this.xenum.Value;
				}
			}

			public XmlSchemaObject Current
			{
				get
				{
					return (XmlSchemaObject)this.xenum.Value;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					return this.xenum.Entry;
				}
			}

			public XmlQualifiedName Key
			{
				get
				{
					return (XmlQualifiedName)this.xenum.Key;
				}
			}

			public XmlSchemaObject Value
			{
				get
				{
					return (XmlSchemaObject)this.xenum.Value;
				}
			}

			public bool MoveNext()
			{
				return this.xenum.MoveNext();
			}

			private IDictionaryEnumerator xenum;

			private IEnumerable tmp;
		}
	}
}
