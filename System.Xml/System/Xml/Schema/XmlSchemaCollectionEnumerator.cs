using System;
using System.Collections;

namespace System.Xml.Schema
{
	public sealed class XmlSchemaCollectionEnumerator : IEnumerator
	{
		internal XmlSchemaCollectionEnumerator(ICollection col)
		{
			this.xenum = col.GetEnumerator();
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
				return this.xenum.Current;
			}
		}

		public XmlSchema Current
		{
			get
			{
				return (XmlSchema)this.xenum.Current;
			}
		}

		public bool MoveNext()
		{
			return this.xenum.MoveNext();
		}

		private IEnumerator xenum;
	}
}
