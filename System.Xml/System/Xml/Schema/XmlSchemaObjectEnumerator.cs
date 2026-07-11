using System;
using System.Collections;

namespace System.Xml.Schema
{
	public class XmlSchemaObjectEnumerator : IEnumerator
	{
		internal XmlSchemaObjectEnumerator(IList list)
		{
			this.ienum = list.GetEnumerator();
		}

		bool IEnumerator.MoveNext()
		{
			return this.ienum.MoveNext();
		}

		void IEnumerator.Reset()
		{
			this.ienum.Reset();
		}

		object IEnumerator.Current
		{
			get
			{
				return (XmlSchemaObject)this.ienum.Current;
			}
		}

		public XmlSchemaObject Current
		{
			get
			{
				return (XmlSchemaObject)this.ienum.Current;
			}
		}

		public bool MoveNext()
		{
			return this.ienum.MoveNext();
		}

		public void Reset()
		{
			this.ienum.Reset();
		}

		private IEnumerator ienum;
	}
}
