using System;
using System.Collections;

namespace System.Xml.Schema
{
	public class XmlSchemaObjectEnumerator : IEnumerator
	{
		internal XmlSchemaObjectEnumerator(IEnumerator enumerator)
		{
			this.enumerator = enumerator;
		}

		public void Reset()
		{
			this.enumerator.Reset();
		}

		public bool MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		public XmlSchemaObject Current
		{
			get
			{
				return (XmlSchemaObject)this.enumerator.Current;
			}
		}

		void IEnumerator.Reset()
		{
			this.enumerator.Reset();
		}

		bool IEnumerator.MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		object IEnumerator.Current
		{
			get
			{
				return this.enumerator.Current;
			}
		}

		private IEnumerator enumerator;
	}
}
