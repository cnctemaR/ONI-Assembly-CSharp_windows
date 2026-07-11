using System;
using System.Collections;

namespace System.Xml.Schema
{
	public sealed class XmlSchemaCollectionEnumerator : IEnumerator
	{
		internal XmlSchemaCollectionEnumerator(Hashtable collection)
		{
			this.enumerator = collection.GetEnumerator();
		}

		void IEnumerator.Reset()
		{
			this.enumerator.Reset();
		}

		bool IEnumerator.MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		public bool MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		public XmlSchema Current
		{
			get
			{
				XmlSchemaCollectionNode xmlSchemaCollectionNode = (XmlSchemaCollectionNode)this.enumerator.Value;
				if (xmlSchemaCollectionNode != null)
				{
					return xmlSchemaCollectionNode.Schema;
				}
				return null;
			}
		}

		internal XmlSchemaCollectionNode CurrentNode
		{
			get
			{
				return (XmlSchemaCollectionNode)this.enumerator.Value;
			}
		}

		private IDictionaryEnumerator enumerator;
	}
}
